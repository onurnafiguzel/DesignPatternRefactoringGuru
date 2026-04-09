namespace DesignPatterns.Creational.Singleton.WrongApproach;

/// <summary>
/// ❌ KÖTÜ YAKLAŞIM: Pattern olmadan, kontrol dışı örnek oluşturma
///
/// Sorun: Bağlantı pool'unun kontrollü oluşturulmaması.
/// Herhangi yerde new yazılırsa yeni bir pool oluşturulur.
/// </summary>
public class DatabaseConnectionPool
{
    private static List<DatabaseConnectionPool> _instances = new();

    public string PoolId { get; }
    public int ConnectionCount { get; set; }

    // ❌ Public constructor - sınırsız sayıda örnek oluşturulabilir
    public DatabaseConnectionPool()
    {
        PoolId = Guid.NewGuid().ToString();
        ConnectionCount = 0;
        _instances.Add(this);

        Console.WriteLine($"⚠️  YENİ POOL OLUŞTURULDU: {PoolId}");
        Console.WriteLine($"   Toplam pool sayısı: {_instances.Count}");
    }

    public void OpenConnection()
    {
        ConnectionCount++;
        Console.WriteLine($"   [Pool: {PoolId[..8]}...] Bağlantı açıldı. Toplam: {ConnectionCount}");
    }

    public static int GetTotalPoolCount() => _instances.Count;
    public static List<DatabaseConnectionPool> GetAllPools() => _instances;
}

/// <summary>
/// PROBLEM 1: User hizmetleri kendi pool'unu oluşturuyor
/// </summary>
public class UserService
{
    public void GetUser(int id)
    {
        var pool = new DatabaseConnectionPool();  // ❌ Yeni pool!
        pool.OpenConnection();
        Console.WriteLine($"   User {id} getirildi");
    }
}

/// <summary>
/// PROBLEM 2: Order hizmetleri başka pool oluşturuyor
/// </summary>
public class OrderService
{
    public void GetOrder(int id)
    {
        var pool = new DatabaseConnectionPool();  // ❌ Başka yeni pool!
        pool.OpenConnection();
        Console.WriteLine($"   Order {id} getirildi");
    }
}

/// <summary>
/// PROBLEM 3: Payment hizmetleri başka pool oluşturuyor
/// </summary>
public class PaymentService
{
    public void ProcessPayment(decimal amount)
    {
        var pool = new DatabaseConnectionPool();  // ❌ Üçüncü pool!
        pool.OpenConnection();
        Console.WriteLine($"   Ödeme işlendi: {amount:C}");
    }
}

public class WrongApproachDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        ❌ SINGLETON PATTERN OLMADAN (Kötü Yaklaşım)      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        var userService = new UserService();
        var orderService = new OrderService();
        var paymentService = new PaymentService();

        Console.WriteLine("--- Hizmetler çalışıyor ---\n");
        userService.GetUser(1);
        orderService.GetOrder(100);
        paymentService.ProcessPayment(99.99m);

        Console.WriteLine("\n--- Sonuç ---");
        Console.WriteLine($"❌ Toplam oluşturulan pool sayısı: {DatabaseConnectionPool.GetTotalPoolCount()}");
        Console.WriteLine("❌ Üç farklı pool, kontrol dışı, yönetim zor!\n");

        Console.WriteLine("SORUNLAR:");
        Console.WriteLine("1. ⚠️  Kaç pool olduğunu kimse bilmiyor");
        Console.WriteLine("2. ⚠️  Bağlantı yönetimi karmaşık ve hata yapmaya açık");
        Console.WriteLine("3. ⚠️  Her hizmet kendi pool'unu bilmeli (tight coupling)");
        Console.WriteLine("4. ⚠️  Test yazmak zor (kaç pool var?)");
        Console.WriteLine("5. ⚠️  Concurrent erişimde race condition'lar\n");
    }
}
