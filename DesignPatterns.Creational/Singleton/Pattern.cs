namespace DesignPatterns.Creational.Singleton.Correct;

/// <summary>
/// ✅ SINGLETON PATTERN: Thread-safe, Lazy Initialization
///
/// Amaç:
/// - Bir sınıfın sadece bir örneğinin olmasını garanti et
/// - Bu örneğe global erişim sağla
/// - Thread-safe yap
/// - Oluşturmayı kontrol et
///
/// Kullanım Alanları:
/// - Database Connection Pool ← Örneğimiz
/// - Logger Singleton
/// - Configuration Manager
/// - Thread Pool
/// - Cache Manager
/// - Print Spooler
/// </summary>
public sealed class DatabaseConnectionPool
{
    // ✅ Private static field: Sadece bir kez oluşturulur
    // ✅ Lazy<T>: Thread-safe initialization + Lazy loading
    // ✅ Double-checked locking otomatik
    private static readonly Lazy<DatabaseConnectionPool> _instance =
        new(() => new DatabaseConnectionPool());

    // Instance'a erişmek için tek yol
    public static DatabaseConnectionPool Instance => _instance.Value;

    // Örnek özellikleri
    public string PoolId { get; }
    public int MaxConnections { get; set; } = 10;
    public int ActiveConnections { get; private set; }

    private List<string> _connectionIds = new();
    private readonly object _lockObject = new object();

    /// <summary>
    /// ✅ Private Constructor: Dışarıdan new ile oluşturulamaz
    /// Sadece Lazy<T> içinden çağrılır
    /// </summary>
    private DatabaseConnectionPool()
    {
        PoolId = Guid.NewGuid().ToString();
        ActiveConnections = 0;

        Console.WriteLine($"✅ SINGLETON POOL OLUŞTURULDU: {PoolId[..8]}...");
        Console.WriteLine($"   (Bu mesaj sadece 1 kez gösterilecek)\n");
    }

    /// <summary>
    /// Bağlantı aç (thread-safe)
    /// </summary>
    public string OpenConnection()
    {
        lock (_lockObject)
        {
            if (ActiveConnections >= MaxConnections)
            {
                throw new InvalidOperationException(
                    $"Pool limit aşıldı. Maks: {MaxConnections}, Aktif: {ActiveConnections}");
            }

            var connectionId = Guid.NewGuid().ToString();
            _connectionIds.Add(connectionId);
            ActiveConnections++;

            Console.WriteLine($"   [✓] Bağlantı açıldı: {connectionId[..8]}...");
            Console.WriteLine($"       Aktif bağlantılar: {ActiveConnections}/{MaxConnections}");

            return connectionId;
        }
    }

    /// <summary>
    /// Bağlantı kapat (thread-safe)
    /// </summary>
    public void CloseConnection(string connectionId)
    {
        lock (_lockObject)
        {
            if (_connectionIds.Remove(connectionId))
            {
                ActiveConnections--;
                Console.WriteLine($"   [✓] Bağlantı kapatıldı: {connectionId[..8]}...");
                Console.WriteLine($"       Kalan bağlantılar: {ActiveConnections}/{MaxConnections}");
            }
        }
    }

    /// <summary>
    /// Pool durumunu kontrol et
    /// </summary>
    public int GetActiveConnectionCount()
    {
        lock (_lockObject)
        {
            return ActiveConnections;
        }
    }

    /// <summary>
    /// ✅ sealed: Singleton'ı kalıtlanamaz yap
    /// Eğer sealed olmasaydı:
    /// public class FakeDatabaseConnectionPool : DatabaseConnectionPool { }
    /// Bu Singleton'ı kırabilir!
    /// </summary>
}

/// <summary>
/// DOĞRU YAKLAŞIM: Tüm hizmetler aynı pool örneğini kullanıyor
/// </summary>
public class UserService
{
    public void GetUser(int id)
    {
        var pool = DatabaseConnectionPool.Instance;  // ✅ Aynı instance!
        var connId = pool.OpenConnection();
        try
        {
            Console.WriteLine($"   → User {id} veritabanından getirildi");
        }
        finally
        {
            pool.CloseConnection(connId);
        }
    }
}

public class OrderService
{
    public void GetOrder(int id)
    {
        var pool = DatabaseConnectionPool.Instance;  // ✅ Aynı instance!
        var connId = pool.OpenConnection();
        try
        {
            Console.WriteLine($"   → Order {id} veritabanından getirildi");
        }
        finally
        {
            pool.CloseConnection(connId);
        }
    }
}

public class PaymentService
{
    public void ProcessPayment(decimal amount)
    {
        var pool = DatabaseConnectionPool.Instance;  // ✅ Aynı instance!
        var connId = pool.OpenConnection();
        try
        {
            Console.WriteLine($"   → Ödeme işlendi: {amount:C}");
        }
        finally
        {
            pool.CloseConnection(connId);
        }
    }
}

public class SingletonPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         ✅ SINGLETON PATTERN (Doğru Yaklaşım)           ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // İlk referans - Instance oluşturulur
        var pool1 = DatabaseConnectionPool.Instance;
        Console.WriteLine($"Pool1 ID: {pool1.PoolId[..8]}...\n");

        // Sonraki referanslar - Aynı instance döner
        var pool2 = DatabaseConnectionPool.Instance;
        var pool3 = DatabaseConnectionPool.Instance;

        // Tümü aynı mı?
        Console.WriteLine($"✓ pool1 == pool2: {ReferenceEquals(pool1, pool2)}");
        Console.WriteLine($"✓ pool1 == pool3: {ReferenceEquals(pool1, pool3)}");
        Console.WriteLine($"✓ Hepsi aynı örnek: {ReferenceEquals(pool1, pool2) && ReferenceEquals(pool2, pool3)}\n");

        // Hizmetler tarafından kullanım
        var userService = new UserService();
        var orderService = new OrderService();
        var paymentService = new PaymentService();

        Console.WriteLine("--- Hizmetler çalışıyor (aynı pool'u kullanıyor) ---\n");
        userService.GetUser(1);
        orderService.GetOrder(100);
        paymentService.ProcessPayment(99.99m);

        Console.WriteLine("\n--- Kontrollü Sonuç ---");
        Console.WriteLine($"✅ Pool sayısı: 1 (tam kontrol!)");
        Console.WriteLine($"✅ Aktif bağlantılar: {pool1.GetActiveConnectionCount()}");
        Console.WriteLine($"✅ Tüm hizmetler aynı pool'u kullanıyor\n");

        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("1. ✅ Tek örnek garantisi - Tam kontrol");
        Console.WriteLine("2. ✅ Thread-safe (lock + Lazy<T>)");
        Console.WriteLine("3. ✅ Lazy initialization (ilk kullanımda oluşturulur)");
        Console.WriteLine("4. ✅ Global erişim noktası (Instance property)");
        Console.WriteLine("5. ✅ Kaynak yönetimi kolay ve etkili\n");
    }
}
