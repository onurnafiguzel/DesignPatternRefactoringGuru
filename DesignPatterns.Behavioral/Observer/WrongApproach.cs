namespace DesignPatterns.Behavioral.Observer.WrongApproach;

/// <summary>
/// ❌ KÖTÜ YAKLAŞIM: Tight Coupling ve Polling
///
/// Sorun: Hisse senedi fiyatı değiştiğinde, birden fazla bileşenin
/// bunu bilmesi ve harekete geçmesi gerekiyor. Ama Stock'un
/// bu bileşenlerin detaylarını bilmesi gerekir mi?
/// </summary>

// Hizmetler (Backend Services)
public class Portfolio
{
    private decimal _totalValue = 0;

    public void UpdatePortfolioValue(decimal newStockPrice)
    {
        _totalValue = newStockPrice * 100;  // 100 hisse var
        Console.WriteLine($"   💼 Portfolio güncellendi. Yeni değer: {_totalValue:C}");
    }

    public decimal GetTotalValue() => _totalValue;
}

public class AlertService
{
    private decimal _priceAlertThreshold = 5m;

    public void CheckAndAlert(string symbol, decimal oldPrice, decimal newPrice)
    {
        var priceChange = Math.Abs(newPrice - oldPrice);
        if (priceChange >= _priceAlertThreshold)
        {
            Console.WriteLine($"   🚨 UYARI: {symbol} büyük fiyat hareketi!");
            Console.WriteLine($"      Eski: {oldPrice:C}, Yeni: {newPrice:C}, Değişim: {priceChange:C}");
        }
    }
}

public class ReportGenerator
{
    public void LogPriceChange(string symbol, decimal oldPrice, decimal newPrice)
    {
        Console.WriteLine($"   📄 Rapor: {symbol} fiyatı {oldPrice:C}'den {newPrice:C}'ye değişti");
    }
}

/// <summary>
/// ❌ PROBLEM: Stock'un tüm hizmetleri bilmesi gerekir
/// </summary>
public class Stock
{
    private string _symbol;
    private decimal _price;

    // ❌ Tight Coupling: Tüm hizmetler burada
    private Portfolio _portfolio;
    private AlertService _alertService;
    private ReportGenerator _reportGenerator;

    // ❌ Stock constructor'da tüm bağımlılıkları almalı
    public Stock(string symbol, decimal initialPrice,
        Portfolio portfolio, AlertService alertService, ReportGenerator reportGenerator)
    {
        _symbol = symbol;
        _price = initialPrice;
        _portfolio = portfolio;
        _alertService = alertService;
        _reportGenerator = reportGenerator;

        Console.WriteLine($"🏢 Stock oluşturuldu: {symbol} @ {initialPrice:C}");
        Console.WriteLine($"   Portfolio: {portfolio}");
        Console.WriteLine($"   AlertService: {alertService}");
        Console.WriteLine($"   ReportGenerator: {reportGenerator}");
        Console.WriteLine("   (Tüm bağımlılıkları bilmeliyiz!)\n");
    }

    public decimal Price
    {
        get { return _price; }
        set
        {
            if (_price != value)
            {
                decimal oldPrice = _price;
                _price = value;

                // ❌ PROBLEM: Stock'un tüm hizmetleri manuel olarak çağırması gerekir
                Console.WriteLine($"\n📊 {_symbol} fiyatı değişti: {oldPrice:C} → {_price:C}\n");

                // Her hizmet için ayrı çağrı
                _portfolio.UpdatePortfolioValue(_price);
                _alertService.CheckAndAlert(_symbol, oldPrice, _price);
                _reportGenerator.LogPriceChange(_symbol, oldPrice, _price);

                // ❌ Yeni hizmet (Email, Slack, Database) eklemek istersek?
                // Stock'u değiştirmek gerekir!
            }
        }
    }

    public string Symbol => _symbol;
}

/// <summary>
/// ❌ PROBLEM: Yeni hizmet eklemek gerekirse (örn. EmailNotifier)
/// </summary>
public class EmailNotifier
{
    public void SendEmail(string symbol, decimal newPrice)
    {
        Console.WriteLine($"   📧 Email gönderiliyor: {symbol} fiyatı {newPrice:C}");
    }
}

/// <summary>
/// ❌ PROBLEM: Stock'u değiştirmek gerekir
/// </summary>
public class ExtendedStock
{
    private string _symbol;
    private decimal _price;

    // ❌ Daha fazla bağımlılık
    private Portfolio _portfolio;
    private AlertService _alertService;
    private ReportGenerator _reportGenerator;
    private EmailNotifier _emailNotifier;  // Yeni

    public ExtendedStock(string symbol, decimal initialPrice,
        Portfolio portfolio, AlertService alertService,
        ReportGenerator reportGenerator, EmailNotifier emailNotifier)  // Yeni param
    {
        _symbol = symbol;
        _price = initialPrice;
        _portfolio = portfolio;
        _alertService = alertService;
        _reportGenerator = reportGenerator;
        _emailNotifier = emailNotifier;  // Bağla
    }

    public decimal Price
    {
        get { return _price; }
        set
        {
            if (_price != value)
            {
                decimal oldPrice = _price;
                _price = value;

                Console.WriteLine($"\n📊 {_symbol} fiyatı değişti: {oldPrice:C} → {_price:C}\n");

                // ❌ Çağrı sayısı artıyor
                _portfolio.UpdatePortfolioValue(_price);
                _alertService.CheckAndAlert(_symbol, oldPrice, _price);
                _reportGenerator.LogPriceChange(_symbol, oldPrice, _price);
                _emailNotifier.SendEmail(_symbol, _price);  // Yeni çağrı

                // Eğer Slack, Database, Analytics eklemek istersek?
                // Daha fazla değişim!
            }
        }
    }

    public string Symbol => _symbol;
}

/// <summary>
/// ❌ PROBLEM: Polling (Aktif Kontrol)
/// </summary>
public class PollingExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Polling Sorunu ---\n");

        var stock = new Stock("GOOGL", 2800m,
            new Portfolio(), new AlertService(), new ReportGenerator());

        // ❌ İlgili hizmetler fiyatı sürekli kontrol etmeliyse?
        // while (true)
        // {
        //     // 1 saniyede bir kontrol et
        //     if (stock.Price değişti mi?)
        //     {
        //         // Harekete geç
        //     }
        //     Thread.Sleep(1000);
        // }

        Console.WriteLine("⚠️  Polling Sorunları:");
        Console.WriteLine("   1. CPU tüketimi (sürekli kontrol)");
        Console.WriteLine("   2. Delay'ler (1 saniye sonra fark edebilir)");
        Console.WriteLine("   3. Inefficient (boş kontroller)");
        Console.WriteLine("   4. Scalable değil\n");
    }
}

public class WrongApproachDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║    ❌ OBSERVER PATTERN OLMADAN (Kötü Yaklaşım)          ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // Hizmetleri oluştur
        var portfolio = new Portfolio();
        var alertService = new AlertService();
        var reportGenerator = new ReportGenerator();

        // ❌ PROBLEM 1: Stock constructor'da tüm hizmetleri geçmek gerekir
        Console.WriteLine("PROBLEM 1: Stock Constructor'ı Karmaşıklaşıyor\n");
        var stock1 = new Stock("APPLE", 150m, portfolio, alertService, reportGenerator);

        // ❌ PROBLEM 2: Fiyat değişti, tüm hizmetler otomatik harekete geçer
        Console.WriteLine("--- Fiyat Değişiyor ---");
        stock1.Price = 155m;

        Console.WriteLine("\n--- Daha Büyük Değişim ---");
        stock1.Price = 165m;  // Alert tetiklenecek

        Console.WriteLine("\n\n" + new string('─', 60));
        Console.WriteLine("PROBLEM 2: Yeni Hizmet Eklemek Gerekirse\n");

        var emailNotifier = new EmailNotifier();

        // ❌ ExtendedStock oluşturmak gerekti
        Console.WriteLine("EmailNotifier ekledik, ama Stock'u değiştirmek gerekti!");
        var stock2 = new ExtendedStock("GOOGL", 2800m,
            portfolio, alertService, reportGenerator, emailNotifier);

        Console.WriteLine("\n--- Fiyat Değişiyor ---");
        stock2.Price = 2850m;

        Console.WriteLine("\n\n" + new string('─', 60));
        Console.WriteLine("SORUNLAR ÖZET:\n");
        Console.WriteLine("❌ TIGHT COUPLING");
        Console.WriteLine("   Stock tüm hizmetleri direkt bilmeli");
        Console.WriteLine("   Stock constructor'ında tüm bağımlılıkları geçmek gerekir\n");

        Console.WriteLine("❌ SCALABILITY");
        Console.WriteLine("   Yeni hizmet eklemek → ExtendedStock oluşturmak");
        Console.WriteLine("   Stock'u her seferinde değiştirmek gerekir\n");

        Console.WriteLine("❌ RESPONSIBILITY");
        Console.WriteLine("   Stock sadece fiyat yönetmemeli");
        Console.WriteLine("   Ama tüm hizmetleri çağırmalı!\n");

        Console.WriteLine("❌ TESTABILITY");
        Console.WriteLine("   Stock test yazmak zor");
        Console.WriteLine("   Tüm hizmetleri mock'lamalı\n");

        Console.WriteLine("❌ OPEN/CLOSED PRINCIPLE");
        Console.WriteLine("   Stock'u değiştirmeden yeni hizmet eklenemez\n");

        PollingExample.Run();
    }
}
