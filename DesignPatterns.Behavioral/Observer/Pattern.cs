namespace DesignPatterns.Behavioral.Observer.Correct;

/// <summary>
/// ✅ OBSERVER PATTERN: Loose Coupling ile Event-Driven
///
/// Amaç:
/// - Nesneler arası loose coupling sağla
/// - Push-based notification (polling değil)
/// - Runtime'da dinamik subscribe/unsubscribe
/// - Her bileşenin bağımsız çalışması
///
/// Kullanım Alanları:
/// - Event-driven systems
/// - GUI event handling
/// - Real-time updates
/// - Publish-Subscribe systems
/// - Stock price notifications ← Örneğimiz
/// - Sensor monitoring
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Observer Interface (Sözleşme)
// ═══════════════════════════════════════════════════════════

public interface IStockObserver
{
    void OnPriceChanged(StockPriceChangedEventArgs args);
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Event Arguments (Veri Transferi)
// ═══════════════════════════════════════════════════════════

public class StockPriceChangedEventArgs
{
    public string Symbol { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime ChangedAt { get; set; }

    public StockPriceChangedEventArgs(string symbol, decimal oldPrice, decimal newPrice)
    {
        Symbol = symbol;
        OldPrice = oldPrice;
        NewPrice = newPrice;
        ChangedAt = DateTime.UtcNow;
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Subject (Observer'ları Yönet)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ Stock: Sadece fiyat yönetir, observer'lar ekler/çıkarır
/// </summary>
public class Stock
{
    private string _symbol;
    private decimal _price;

    // ✅ Observer listesi (hizmetleri bilmiyor, interface'i biliyor)
    private List<IStockObserver> _observers = new();

    public string Symbol { get; }
    public decimal Price
    {
        get { return _price; }
        set
        {
            if (_price != value)
            {
                var oldPrice = _price;
                _price = value;
                NotifyObservers(oldPrice);
            }
        }
    }

    public Stock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
        Console.WriteLine($"📈 Stock oluşturuldu: {symbol} @ {initialPrice:C}");
    }

    // ✅ Observer subscribe eder
    public void Subscribe(IStockObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"   [+] {observer.GetType().Name} subscribe oldu");
    }

    // ✅ Observer unsubscribe eder
    public void Unsubscribe(IStockObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"   [-] {observer.GetType().Name} unsubscribe oldu");
    }

    // ✅ Fiyat değiştiğinde tüm observer'lara bildir
    private void NotifyObservers(decimal oldPrice)
    {
        var args = new StockPriceChangedEventArgs(Symbol, oldPrice, _price);

        Console.WriteLine($"\n📊 {Symbol} fiyatı değişti: {oldPrice:C} → {_price:C}");
        Console.WriteLine($"   ({Math.Abs(_price - oldPrice):C} değişim)\n");

        foreach (var observer in _observers)
        {
            observer.OnPriceChanged(args);
        }
    }

    public int GetObserverCount() => _observers.Count;
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Concrete Observers (Hizmetler)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Observer 1: Portföy yönetimi
/// </summary>
public class Portfolio : IStockObserver
{
    private string _name;
    private Dictionary<string, int> _holdings = new();  // Stock -> Adet

    public Portfolio(string name)
    {
        _name = name;
    }

    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        var priceChange = args.NewPrice - args.OldPrice;
        var changePercent = (priceChange / args.OldPrice) * 100;

        Console.WriteLine($"   💼 Portfolio '{_name}':");
        Console.WriteLine($"      {args.Symbol} = {args.NewPrice:C}");
        Console.WriteLine($"      Değişim: {changePercent:+0.00;-0.00}%");
    }
}

/// <summary>
/// Observer 2: Uyarı servisi (büyük değişimleri kontrol et)
/// </summary>
public class AlertService : IStockObserver
{
    private decimal _alertThreshold;

    public AlertService(decimal alertThreshold)
    {
        _alertThreshold = alertThreshold;
    }

    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        var priceChange = Math.Abs(args.NewPrice - args.OldPrice);

        if (priceChange >= _alertThreshold)
        {
            Console.WriteLine($"   🚨 UYARI AlertService:");
            Console.WriteLine($"      {args.Symbol} büyük fiyat hareketi!");
            Console.WriteLine($"      Değişim: {priceChange:C} (Eşik: {_alertThreshold:C})");
        }
        else
        {
            Console.WriteLine($"   ℹ️  AlertService:");
            Console.WriteLine($"      {args.Symbol} değişim eşiğin altında ({priceChange:C})");
        }
    }
}

/// <summary>
/// Observer 3: Rapor oluşturucu
/// </summary>
public class ReportGenerator : IStockObserver
{
    private List<string> _report = new();

    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        var entry = $"{args.ChangedAt:yyyy-MM-dd HH:mm:ss} | {args.Symbol} | " +
                    $"{args.OldPrice:C} → {args.NewPrice:C}";
        _report.Add(entry);

        Console.WriteLine($"   📄 ReportGenerator:");
        Console.WriteLine($"      Rapor kaydedildi ({_report.Count} girdi)");
    }

    public void PrintReport()
    {
        Console.WriteLine("\n=== FİYAT DEĞİŞİMİ RAPORU ===");
        foreach (var entry in _report)
        {
            Console.WriteLine($"  {entry}");
        }
    }
}

/// <summary>
/// Observer 4: Email Notifier (yeni observer)
/// </summary>
public class EmailNotifier : IStockObserver
{
    private string _email;

    public EmailNotifier(string email)
    {
        _email = email;
    }

    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        var changePercent = ((args.NewPrice - args.OldPrice) / args.OldPrice) * 100;

        Console.WriteLine($"   📧 EmailNotifier:");
        Console.WriteLine($"      {_email} adresine e-posta gönderiliyor");
        Console.WriteLine($"      Konu: {args.Symbol} fiyatı {changePercent:+0.00;-0.00}% değişti");
    }
}

/// <summary>
/// Observer 5: Slack Notifier (yeni observer)
/// </summary>
public class SlackNotifier : IStockObserver
{
    private string _channel;

    public SlackNotifier(string channel)
    {
        _channel = channel;
    }

    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        Console.WriteLine($"   💬 SlackNotifier:");
        Console.WriteLine($"      #{_channel} kanalına mesaj gönderiliyor");
        Console.WriteLine($"      {args.Symbol}: {args.OldPrice:C} → {args.NewPrice:C}");
    }
}

public class ObserverPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         ✅ OBSERVER PATTERN (Doğru Yaklaşım)            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // ✅ Subject oluştur
        var stock = new Stock("APPLE", 150m);

        // ✅ Observer'ları oluştur (Stock'un constructor'ında DEĞIL!)
        var portfolio = new Portfolio("Growth Portfolio");
        var alertService = new AlertService(5m);  // 5 TL'lik harekete uyar
        var reportGenerator = new ReportGenerator();
        var emailNotifier = new EmailNotifier("investor@example.com");
        var slackNotifier = new SlackNotifier("stock-alerts");

        // ✅ Observer'ları subscribe et
        Console.WriteLine("\n--- Observer'lar Subscribe Ediliyor ---\n");
        stock.Subscribe(portfolio);
        stock.Subscribe(alertService);
        stock.Subscribe(reportGenerator);
        stock.Subscribe(emailNotifier);
        stock.Subscribe(slackNotifier);

        Console.WriteLine($"\n✓ Toplam Observer: {stock.GetObserverCount()}\n");

        // ✅ Fiyat değişikliği 1
        Console.WriteLine("=== Fiyat Değişikliği 1: Küçük değişim ===");
        stock.Price = 152m;

        // ✅ Fiyat değişikliği 2
        Console.WriteLine("\n=== Fiyat Değişikliği 2: Büyük değişim ===");
        stock.Price = 165m;  // Alert tetiklenecek

        // ✅ Observer'ları dinamik olarak çıkar
        Console.WriteLine("\n--- AlertService Unsubscribe Ediliyor ---\n");
        stock.Unsubscribe(alertService);

        // ✅ Fiyat değişikliği 3 (AlertService görmez)
        Console.WriteLine("=== Fiyat Değişikliği 3: Büyük değişim (AlertService yok) ===");
        stock.Price = 160m;

        // ✅ Raporu yazdır
        reportGenerator.PrintReport();

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ Stock sadece fiyat yönetiyor");
        Console.WriteLine("✓ Observer'lar bağımsız çalışıyor");
        Console.WriteLine("✓ Yeni observer eklemek kolay");
        Console.WriteLine("✓ Runtime'da subscribe/unsubscribe");
        Console.WriteLine("✓ Loose coupling");
        Console.WriteLine("✓ Push-based (polling yok)");
        Console.WriteLine("✓ Open/Closed Principle ✓\n");
    }
}
