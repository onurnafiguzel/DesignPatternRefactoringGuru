namespace DesignPatterns.Structural.Decorator.Correct;

/// <summary>
/// ✅ DECORATOR PATTERN: Nesneyi sararak davranış ekle
///
/// Amaç:
/// - Nesnelere dinamik olarak sorumluluk ekle
/// - Alt sınıf patlaması olmadan kombinasyonlar kur
/// - Runtime'da katmanları ekle / çıkar
/// - Her decorator tek bir sorumluluğa sahip
///
/// Kaynak: https://refactoring.guru/design-patterns/decorator
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Component Interface
// ═══════════════════════════════════════════════════════════

public interface IDataSource
{
    void WriteData(string data);
    string ReadData();
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Concrete Component (Temel bileşen, sarılan nesne)
// ═══════════════════════════════════════════════════════════

public class FileDataSource : IDataSource
{
    private readonly string _filename;
    private string _storage = "";

    public FileDataSource(string filename)
        => _filename = filename;

    public void WriteData(string data)
    {
        _storage = data;
        Console.WriteLine($"   💾 Dosyaya yazıldı [{_filename}]: \"{Truncate(data)}\"");
    }

    public string ReadData()
    {
        Console.WriteLine($"   💾 Dosyadan okundu [{_filename}]");
        return _storage;
    }

    private static string Truncate(string s) => s.Length > 30 ? s[..30] + "…" : s;
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Base Decorator (Sarma mantığını barındırır)
// ═══════════════════════════════════════════════════════════

public abstract class DataSourceDecorator : IDataSource
{
    // ✅ Sarılan nesneyi tutar — aynı interface
    protected readonly IDataSource _wrappee;

    protected DataSourceDecorator(IDataSource wrappee)
        => _wrappee = wrappee;

    // Varsayılan: doğrudan delege et; alt sınıflar override eder
    public virtual void WriteData(string data) => _wrappee.WriteData(data);
    public virtual string ReadData()           => _wrappee.ReadData();
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Concrete Decorators
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Şifreleme katmanı: yazar önce şifreler, okurken çözer
/// </summary>
public class EncryptionDecorator : DataSourceDecorator
{
    public EncryptionDecorator(IDataSource wrappee) : base(wrappee) { }

    public override void WriteData(string data)
    {
        var encrypted = Encrypt(data);
        Console.WriteLine($"   🔐 Şifrelendi: \"{Truncate(data)}\" → \"{Truncate(encrypted)}\"");
        _wrappee.WriteData(encrypted);         // Şifrelenmiş veriyi bir alta ver
    }

    public override string ReadData()
    {
        var data = _wrappee.ReadData();        // Alttaki katmandan al
        var decrypted = Decrypt(data);
        Console.WriteLine($"   🔓 Şifre çözüldü");
        return decrypted;
    }

    // Basit demo şifreleme (gerçekte AES kullanılır)
    private static string Encrypt(string data) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));

    private static string Decrypt(string data) =>
        System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));

    private static string Truncate(string s) => s.Length > 25 ? s[..25] + "…" : s;
}

/// <summary>
/// Sıkıştırma katmanı: yazar önce sıkıştırır, okurken açar
/// </summary>
public class CompressionDecorator : DataSourceDecorator
{
    public CompressionDecorator(IDataSource wrappee) : base(wrappee) { }

    public override void WriteData(string data)
    {
        var compressed = Compress(data);
        Console.WriteLine($"   📦 Sıkıştırıldı: {data.Length} → {compressed.Length} byte");
        _wrappee.WriteData(compressed);
    }

    public override string ReadData()
    {
        var data = _wrappee.ReadData();
        var decompressed = Decompress(data);
        Console.WriteLine($"   📂 Açıldı: {data.Length} → {decompressed.Length} byte");
        return decompressed;
    }

    // Demo sıkıştırma
    private static string Compress(string data) =>
        $"[COMPRESSED:{data.Length}]{data[..Math.Min(10, data.Length)]}";

    private static string Decompress(string data)
    {
        // Demo için orijinal veriyi simüle et
        var match = System.Text.RegularExpressions.Regex.Match(data, @"\[COMPRESSED:(\d+)\](.+)");
        return match.Success ? $"(decompressed content of {match.Groups[1].Value} bytes)" : data;
    }
}

/// <summary>
/// Önbellek katmanı: aynı veri tekrar sorulursa kaynağa gitme
/// </summary>
public class CacheDecorator : DataSourceDecorator
{
    private string? _cachedData;

    public CacheDecorator(IDataSource wrappee) : base(wrappee) { }

    public override void WriteData(string data)
    {
        _cachedData = null;         // Yazma olursa cache'i temizle
        _wrappee.WriteData(data);
    }

    public override string ReadData()
    {
        if (_cachedData is not null)
        {
            Console.WriteLine("   ⚡ Cache'den döndürüldü (kaynak okunmadı)");
            return _cachedData;
        }

        _cachedData = _wrappee.ReadData();
        Console.WriteLine("   📥 Cache'e alındı");
        return _cachedData;
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class DecoratorPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║       ✅ DECORATOR PATTERN — Veri Akışı Katmanları      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        var payload = "Kullanıcı şifresi: SuperSecret123!";

        // ─── Senaryo 1: Sadece dosya ───
        Console.WriteLine("\n─── Katman 1: Temel Dosya ───");
        IDataSource source = new FileDataSource("data.bin");
        source.WriteData(payload);

        // ─── Senaryo 2: Şifreli dosya ───
        Console.WriteLine("\n─── Katman 2: Şifreli Dosya ───");
        source = new EncryptionDecorator(
                     new FileDataSource("data.bin"));
        source.WriteData(payload);

        // ─── Senaryo 3: Şifreli + Sıkıştırılmış ───
        Console.WriteLine("\n─── Katman 3: Sıkıştırılmış + Şifreli (sarma sırası önemli!) ───");
        source = new CompressionDecorator(       // En dış: önce sıkıştır
                     new EncryptionDecorator(    // Sonra şifrele
                         new FileDataSource("data.bin")));
        Console.WriteLine("  Yazma (dıştan içe: sıkıştır → şifrele → kaydet):");
        source.WriteData(payload);

        Console.WriteLine("\n  Okuma (içten dışa: oku → şifre çöz → aç):");
        source.ReadData();

        // ─── Senaryo 4: Cache katmanı eklendi ───
        Console.WriteLine("\n─── Katman 4: Cache + Sıkıştırma + Şifreleme ───");
        source = new CacheDecorator(
                     new CompressionDecorator(
                         new EncryptionDecorator(
                             new FileDataSource("data.bin"))));
        source.WriteData(payload);
        Console.WriteLine("\n  1. okuma (cache yok):");
        source.ReadData();
        Console.WriteLine("\n  2. okuma (cache var):");
        source.ReadData();

        // ─── Runtime konfigürasyon ───
        Console.WriteLine("\n─── Runtime'da Konfigürasyon ───");
        bool encryptEnabled  = true;
        bool compressEnabled = false;

        IDataSource configured = new FileDataSource("config.bin");
        if (encryptEnabled)  configured = new EncryptionDecorator(configured);
        if (compressEnabled) configured = new CompressionDecorator(configured);

        Console.WriteLine($"Encrypt={encryptEnabled}, Compress={compressEnabled}:");
        configured.WriteData("config verisi");

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ 3 özellik = 3 sınıf, alt sınıf patlaması yok");
        Console.WriteLine("✓ Sarma sırası = işlem sırası (dıştan içe yaz, içten dışa oku)");
        Console.WriteLine("✓ Runtime'da koşula göre katman ekle/çıkar");
        Console.WriteLine("✓ ASP.NET Core middleware pipeline tam olarak bu pattern\n");
    }
}
