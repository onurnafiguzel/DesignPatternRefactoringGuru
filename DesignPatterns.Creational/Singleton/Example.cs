namespace DesignPatterns.Creational.Singleton.Examples;

using DesignPatterns.Creational.Singleton.Correct;
using DesignPatterns.Creational.Singleton.WrongApproach;

/// <summary>
/// Singleton Pattern ve olmadan yaklaşımının karşılaştırmalı örneği
/// </summary>
public class SingletonComparison
{
    public static void RunAll()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("    SINGLETON PATTERN - KARŞILAŞTIRMALI ÖRNEK");
        Console.WriteLine(new string('═', 70));

        // Kötü yaklaşımı göster
        RunWrongApproach();

        // Doğru yaklaşımı göster
        RunCorrectApproach();

        // Karşılaştırma tablosu
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
        Correct.SingletonPatternDemo.Run();

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
            ("Özellik", "PATTERN OLMADAN", "SINGLETON PATTERN"),
            ("─────────────────", "─────────────────", "─────────────────"),
            ("Instance Sayısı", "Sınırsız ❌", "1 ✅"),
            ("Kontrol", "Yok ❌", "Tam ✅"),
            ("Thread-Safe", "Hayır ❌", "Evet ✅"),
            ("Kaynak Yönetimi", "Zor ❌", "Kolay ✅"),
            ("Testlenebilirlik", "Zor ❌", "Orta ✅"),
            ("Global Erişim", "Sorunlu ❌", "Uygun ✅"),
            ("Performans", "Düşük ❌", "Yüksek ✅"),
        };

        foreach (var (feature, wrong, correct) in data)
        {
            Console.WriteLine($"  {feature,-20} | {wrong,-20} | {correct,-20}");
        }

        Console.WriteLine("\n" + new string('═', 70));
    }
}

/// <summary>
/// Singleton'ın pratik kullanım senaryosu
/// </summary>
public class PracticalExample
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         PRATIK SENARYO: E-Commerce Uygulaması            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("Senaryo: Tüm hizmetler bir veritabanı pool'u kullanıyor\n");

        // Pool'u al
        var pool = DatabaseConnectionPool.Instance;
        Console.WriteLine($"Ana Pool oluşturuldu: {pool.PoolId[..8]}...\n");

        // Simüle edilen işlemler
        var operations = new List<(string Service, Action Action)>
        {
            ("Kullanıcı Kaydı", () =>
            {
                Console.WriteLine("▶ Kullanıcı Kaydı Servisi başladı");
                var user = new UserService();
                user.GetUser(1001);
            }),

            ("Sipariş Oluşturma", () =>
            {
                Console.WriteLine("\n▶ Sipariş Servisi başladı");
                var order = new OrderService();
                order.GetOrder(5001);
            }),

            ("Ödeme İşlemi", () =>
            {
                Console.WriteLine("\n▶ Ödeme Servisi başladı");
                var payment = new PaymentService();
                payment.ProcessPayment(299.99m);
            }),

            ("İkinci Sipariş", () =>
            {
                Console.WriteLine("\n▶ Sipariş Servisi başladı (2. defa)");
                var order = new OrderService();
                order.GetOrder(5002);
            }),
        };

        foreach (var (serviceName, action) in operations)
        {
            try
            {
                action();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"   ✗ HATA: {ex.Message}");
            }
        }

        // Sonuçları özetle
        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("SONUÇ:");
        Console.WriteLine($"  ✓ Toplam bağlantı açma kapama: 4 işlem");
        Console.WriteLine($"  ✓ Aktif bağlantı: {pool.GetActiveConnectionCount()}");
        Console.WriteLine($"  ✓ Pool ID: {pool.PoolId[..8]}... (Tüm işlemler aynı pool)");
        Console.WriteLine($"  ✓ Kaynak yönetimi: KONTROLLÜ ✅");
        Console.WriteLine(new string('─', 60) + "\n");
    }
}

/// <summary>
/// Singleton testleme örneği
/// </summary>
public class SingletonTesting
{
    /// <summary>
    /// Unit test senaryosu (pseudocode)
    /// </summary>
    public static void RunTests()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            SINGLETON TEST ÖRNEKLERI                      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // Test 1: Tek örnek garantisi
        Console.WriteLine("Test 1: İkinci erişim aynı örneği verir mi?");
        var instance1 = DatabaseConnectionPool.Instance;
        var instance2 = DatabaseConnectionPool.Instance;
        var passed = ReferenceEquals(instance1, instance2);
        Console.WriteLine($"  Result: {(passed ? "✓ PASSED" : "✗ FAILED")}\n");

        // Test 2: Thread-safety
        Console.WriteLine("Test 2: Thread-safety (Concurrent access)");
        var instance3 = DatabaseConnectionPool.Instance;
        var instances = new System.Collections.Concurrent.ConcurrentBag<DatabaseConnectionPool>();

        var threads = new List<System.Threading.Thread>();
        for (int i = 0; i < 10; i++)
        {
            var thread = new System.Threading.Thread(() =>
            {
                instances.Add(DatabaseConnectionPool.Instance);
            });
            threads.Add(thread);
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var allSame = instances.All(x => ReferenceEquals(x, instance3));
        Console.WriteLine($"  Toplam thread: {threads.Count}");
        Console.WriteLine($"  Hepsi aynı örnek: {(allSame ? "✓ PASSED" : "✗ FAILED")}\n");

        // Test 3: Connection Management
        Console.WriteLine("Test 3: Bağlantı yönetimi");
        var pool = DatabaseConnectionPool.Instance;
        var conn1 = pool.OpenConnection();
        var conn2 = pool.OpenConnection();
        Console.WriteLine($"  2 bağlantı açıldı");
        Console.WriteLine($"  Aktif bağlantı: {pool.GetActiveConnectionCount()}");
        Console.WriteLine($"  Expected: 2, Actual: {pool.GetActiveConnectionCount()}");
        Console.WriteLine($"  Result: {(pool.GetActiveConnectionCount() == 2 ? "✓ PASSED" : "✗ FAILED")}\n");

        pool.CloseConnection(conn1);
        pool.CloseConnection(conn2);
        Console.WriteLine($"  2 bağlantı kapatıldı");
        Console.WriteLine($"  Aktif bağlantı: {pool.GetActiveConnectionCount()}");
        Console.WriteLine($"  Result: {(pool.GetActiveConnectionCount() == 0 ? "✓ PASSED" : "✗ FAILED")}\n");
    }
}

/// <summary>
/// Main örnek çağırıcı
/// </summary>
public class SingletonExample
{
    public static void RunAll()
    {
        SingletonComparison.RunAll();
        PracticalExample.Run();
        SingletonTesting.RunTests();

        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("    ÖNEMLİ NOTLAR");
        Console.WriteLine(new string('═', 70));

        Console.WriteLine(@"
1. NE ZAMAN KULLANILIR?
   • Database connection pools
   • Logger instances
   • Configuration managers
   • Cache managers
   • Printer spoolers

2. NE ZAMAN KULLANILMAZ?
   • Stateful nesneler (her istekte farklı state)
   • Unit testing'in zor olacağı durumlar
   • Dependency Injection kullanıldığı yerler

3. MOD ERN YAKLAŞIM
   • Singleton -> DI Container (ASP.NET Core)
   • IServiceCollection.AddSingleton() tercih edin
   • Testlenebilirlik için interface ile sarıp DI yap

4. DIKKAT!
   • Singleton = Global State (kötü kullanımı istemeyiz)
   • Over-use etmeyiniz!
   • Thread-safety çok önemli
");

        Console.WriteLine(new string('═', 70) + "\n");
    }
}
