namespace DesignPatterns.Behavioral.Observer.Examples;

using DesignPatterns.Behavioral.Observer.Correct;
using DesignPatterns.Behavioral.Observer.WrongApproach;

/// <summary>
/// Observer Pattern'in karşılaştırmalı örneği
/// </summary>
public class ObserverComparison
{
    public static void RunAll()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("    OBSERVER PATTERN - KARŞILAŞTIRMALI ÖRNEK");
        Console.WriteLine(new string('═', 70));

        // Kötü yaklaşımı göster
        RunWrongApproach();

        // Doğru yaklaşımı göster
        RunCorrectApproach();

        // Karşılaştırma
        ShowComparison();
    }

    private static void RunWrongApproach()
    {
        WrongApproach.WrongApproachDemo.Run();

        Console.WriteLine("\nDevam etmek için Enter'a basınız...");
        Console.ReadLine();
    }

    private static void RunCorrectApproach()
    {
        Correct.ObserverPatternDemo.Run();

        Console.WriteLine("\nDevam etmek için Enter'a basınız...");
        Console.ReadLine();
    }

    private static void ShowComparison()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("    KARŞILAŞTIRMA TABLOSU");
        Console.WriteLine(new string('═', 70) + "\n");

        var data = new[]
        {
            ("Özellik", "PATTERN OLMADAN", "OBSERVER PATTERN"),
            ("──────────────────────", "────────────────", "──────────────"),
            ("Coupling", "Tight ❌", "Loose ✅"),
            ("Subscribe/Unsubscribe", "Yok ❌", "Dinamik ✅"),
            ("Notification", "Push ✅", "Push ✅"),
            ("Polling", "Gerekli ❌", "Yok ✅"),
            ("Yeni Observer Eklemek", "Kod değiş ❌", "Adapter ekle ✅"),
            ("Subject Sorumluluğu", "Çok ❌", "Tek ✅"),
            ("Testlenebilirlik", "Zor ❌", "Kolay ✅"),
            ("Open/Closed Principle", "Çiğneniyor ❌", "Saklı ✅"),
            ("Constructor Karmaşıklığı", "Yüksek ❌", "Düşük ✅"),
        };

        foreach (var (feature, wrong, correct) in data)
        {
            Console.WriteLine($"  {feature,-25} | {wrong,-20} | {correct,-20}");
        }

        Console.WriteLine("\n" + new string('═', 70));
    }
}

/// <summary>
/// Pratik senaryo: Real-time Stock Price Monitoring
/// </summary>
public class RealTimeStockMonitoringExample
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║    PRATIK SENARYO: Real-time Stock Fiyat İzlemesi      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // Hisse senetleri
        var appleStock = new Stock("APPLE", 150m);
        var googleStock = new Stock("GOOGL", 2800m);
        var teslaStock = new Stock("TSLA", 800m);

        // Observer'lar
        var myPortfolio = new Portfolio("Emlak Yatırım Fonu");
        var alertService = new AlertService(10m);  // 10 TL harekete uyar
        var reportGenerator = new ReportGenerator();
        var emailNotifier = new EmailNotifier("portfolio@example.com");
        var slackNotifier = new SlackNotifier("trading-desk");

        // Tüm hisse senetlerine observer'ları subscribe et
        Console.WriteLine("=== Observer'lar Tüm Hisse Senetlerine Ekleniyor ===\n");
        foreach (var stock in new[] { appleStock, googleStock, teslaStock })
        {
            stock.Subscribe(myPortfolio);
            stock.Subscribe(alertService);
            stock.Subscribe(reportGenerator);
            stock.Subscribe(emailNotifier);
            stock.Subscribe(slackNotifier);
        }

        Console.WriteLine("--- Fiyat İzlemesi Başlıyor ---\n");

        // Senaryo: Market Açıldı
        Console.WriteLine("🔔 SENARYO 1: Market Açılırken\n");
        appleStock.Price = 152m;    // Küçük değişim
        googleStock.Price = 2810m;  // Küçük değişim
        teslaStock.Price = 820m;    // Küçük değişim

        // Senaryo: Haberler Yayınlandı
        Console.WriteLine("\n🔔 SENARYO 2: Olumlu Haberler (Büyük Değişim)\n");
        appleStock.Price = 165m;    // % +10
        googleStock.Price = 2850m;  // % +1.8
        teslaStock.Price = 850m;    // % +6.25

        // Senaryo: Investor Riskten Kaçıyor (AlertService'i çıkar)
        Console.WriteLine("\n🔔 SENARYO 3: AlertService Devre Dışı\n");
        Console.WriteLine("--- AlertService Unsubscribe Ediliyor ---\n");
        appleStock.Unsubscribe(alertService);
        googleStock.Unsubscribe(alertService);
        teslaStock.Unsubscribe(alertService);

        appleStock.Price = 160m;  // Düşüş ama alert gelmez

        // Rapor
        reportGenerator.PrintReport();

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("SONUÇ:");
        Console.WriteLine("✓ Portföy 3 hisse senedini real-time izliyor");
        Console.WriteLine("✓ AlertService dinamik olarak açıp kapatıldı");
        Console.WriteLine("✓ Rapor otomatik tutuldu");
        Console.WriteLine("✓ Email ve Slack bildirimleri gönderildi");
        Console.WriteLine("✓ Hiçbir sınıfı değiştirmeden yeni observer eklenebilir\n");
    }
}

/// <summary>
/// Advanced Example: Sensor Monitoring Sistemi
/// </summary>
public class SensorMonitoringExample
{
    public interface ITemperatureSensor
    {
        void OnTemperatureChanged(decimal oldTemp, decimal newTemp);
    }

    public class TemperatureMonitor
    {
        private decimal _temperature;
        private List<ITemperatureSensor> _observers = new();

        public decimal Temperature
        {
            get { return _temperature; }
            set
            {
                if (_temperature != value)
                {
                    var oldTemp = _temperature;
                    _temperature = value;
                    NotifyObservers(oldTemp);
                }
            }
        }

        public void Subscribe(ITemperatureSensor observer) => _observers.Add(observer);
        public void Unsubscribe(ITemperatureSensor observer) => _observers.Remove(observer);

        private void NotifyObservers(decimal oldTemp)
        {
            Console.WriteLine($"🌡️  Sıcaklık değişti: {oldTemp:F1}°C → {_temperature:F1}°C\n");
            foreach (var observer in _observers)
            {
                observer.OnTemperatureChanged(oldTemp, _temperature);
            }
        }
    }

    public class AirConditioner : ITemperatureSensor
    {
        public void OnTemperatureChanged(decimal oldTemp, decimal newTemp)
        {
            if (newTemp > 28)
            {
                Console.WriteLine("   ❄️  Klima: Sıcak! Soğutma başla");
            }
            else if (newTemp < 20)
            {
                Console.WriteLine("   🔥 Klima: Soğuk! Isıtma başla");
            }
            else
            {
                Console.WriteLine("   ✓ Klima: Uygun sıcaklık");
            }
        }
    }

    public class Alarm : ITemperatureSensor
    {
        public void OnTemperatureChanged(decimal oldTemp, decimal newTemp)
        {
            if (newTemp > 35 || newTemp < 10)
            {
                Console.WriteLine($"   🚨 ALARM: Tehlikeli sıcaklık! {newTemp:F1}°C");
            }
        }
    }

    public class Logger : ITemperatureSensor
    {
        private List<string> _logs = new();

        public void OnTemperatureChanged(decimal oldTemp, decimal newTemp)
        {
            var log = $"[{DateTime.Now:HH:mm:ss}] {oldTemp:F1}°C → {newTemp:F1}°C";
            _logs.Add(log);
            Console.WriteLine($"   📝 Logger: {log}");
        }

        public void PrintLogs()
        {
            Console.WriteLine("\n=== SICAKLIK KAYITLARI ===");
            foreach (var log in _logs)
            {
                Console.WriteLine($"  {log}");
            }
        }
    }

    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║       ADVANCED: Sensor Monitoring Sistemi               ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        var monitor = new TemperatureMonitor();
        var airConditioner = new AirConditioner();
        var alarm = new Alarm();
        var logger = new Logger();

        monitor.Subscribe(airConditioner);
        monitor.Subscribe(alarm);
        monitor.Subscribe(logger);

        Console.WriteLine("--- Sıcaklık Değişimleri ---\n");

        monitor.Temperature = 22m;   // Normal
        monitor.Temperature = 32m;   // Sıcak
        monitor.Temperature = 39m;   // Çok sıcak (alarm!)
        monitor.Temperature = 18m;   // Soğuk
        monitor.Temperature = 8m;    // Çok soğuk (alarm!)
        monitor.Temperature = 25m;   // Normal

        logger.PrintLogs();

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJ: Yeni sensor eklemek kolay");
        Console.WriteLine("         Monitor'ı değiştirmeye gerek yok\n");
    }
}

/// <summary>
/// Main örnek çağırıcı
/// </summary>
public class ObserverExample
{
    public static void RunAll()
    {
        ObserverComparison.RunAll();
        RealTimeStockMonitoringExample.Run();
        SensorMonitoringExample.Run();

        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("    ÖNEMLİ NOTLAR");
        Console.WriteLine(new string('═', 70));

        Console.WriteLine(@"
1. OBSERVER vs EVENT (.NET Events)
   • Observer Pattern: Manuel implement
   • .NET Events: event keyword kullan
   • Modern C# → C# events tercih edin

2. WEAK EVENTS (Memory Leak Kaçınmak İçin)
   • Observer'ları unsubscribe etmezsen: Memory leak!
   • WeakEvent pattern kullan
   • IDisposable implement et

3. EXCEPTION HANDLING
   • Bir observer exception atarsa ne olur?
   • Diğer observer'lar tetiklenecek mi?
   • try-catch ile sarıp devam et

4. THREAD SAFETY
   • Multi-threaded ortamda sorun olabilir
   • lock() ile observer listesini koru
   • ConcurrentBag kullan

5. NE ZAMAN KULLANILIR?
   • Event-driven systems
   • Real-time updates (stock, sensor)
   • MVC/MVVM pattern
   • Publish-subscribe systems
   • UI event handling
   • Reactive programming

6. MOD ERN YAKLAŞIM (.NET)
   • Observer pattern → C# events
   • Event handler → EventHandler<T>
   • Örnek: button.Click += OnButtonClick;

7. ALTERNATİFLER
   • Mediator: Merkezi kontrol
   • Publisher-Subscriber: Message queues
   • Reactive Extensions (Rx): LINQ-style events
");

        Console.WriteLine(new string('═', 70) + "\n");
    }
}
