namespace DesignPatterns.Structural.Flyweight.Correct;

/// <summary>
/// ✅ FLYWEIGHT PATTERN: Paylaşılan veriyi tek kopyada tut
///
/// Amaç:
/// - Çok sayıda benzer nesnenin bellek tüketimini azalt
/// - Değişmez (intrinsic) veriyi paylaş
/// - Değişen (extrinsic) veriyi context nesnede tut
/// - Flyweight Factory ile paylaşımı yönet
///
/// Terimler:
///   Intrinsic state  = Paylaşılan, değişmez veri (Flyweight içinde)
///   Extrinsic state  = Benzersiz, değişebilir veri (Context içinde)
///
/// Kaynak: https://refactoring.guru/design-patterns/flyweight
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Flyweight — Intrinsic (Paylaşılan, değişmez) veri
// ═══════════════════════════════════════════════════════════

public sealed class TransactionType
{
    // ✅ Intrinsic state: tüm aynı tipte işlemler bunları paylaşır
    public string Currency         { get; }
    public string MerchantCategory { get; }
    public string ProcessorName    { get; }
    public decimal InterchangeFee  { get; }   // %
    public decimal SchemesFee      { get; }   // %
    public string ProcessorLogoUrl { get; }   // Büyük string, bir kez saklanır

    public TransactionType(
        string currency,
        string merchantCategory,
        string processorName,
        decimal interchangeFee,
        decimal schemesFee,
        string processorLogoUrl)
    {
        Currency         = currency;
        MerchantCategory = merchantCategory;
        ProcessorName    = processorName;
        InterchangeFee   = interchangeFee;
        SchemesFee       = schemesFee;
        ProcessorLogoUrl = processorLogoUrl;
    }

    public decimal CalculateFee(decimal amount)
        => Math.Round(amount * (InterchangeFee + SchemesFee), 2);

    public override string ToString()
        => $"{ProcessorName}/{Currency}/{MerchantCategory}";
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Flyweight Factory — Paylaşılan örnekleri yönetir
// ═══════════════════════════════════════════════════════════

public class TransactionTypeFactory
{
    // ✅ Cache: aynı kombinasyon için tek flyweight
    private readonly Dictionary<string, TransactionType> _pool = new();

    public TransactionType GetOrCreate(
        string currency,
        string merchantCategory,
        string processorName,
        decimal interchangeFee,
        decimal schemesFee)
    {
        // Bileşik anahtar: tüm intrinsic değerlerin kombinasyonu
        var key = $"{currency}|{merchantCategory}|{processorName}";

        if (!_pool.TryGetValue(key, out var flyweight))
        {
            flyweight = new TransactionType(
                currency, merchantCategory, processorName,
                interchangeFee, schemesFee,
                $"https://cdn.processor.com/logos/{processorName.ToLower()}.png");

            _pool[key] = flyweight;
            Console.WriteLine($"   [Factory] Yeni flyweight oluşturuldu: {key}");
        }

        return flyweight;
    }

    public int PoolSize => _pool.Count;

    public void PrintPool()
    {
        Console.WriteLine($"\n   Flyweight Pool ({_pool.Count} benzersiz tip):");
        foreach (var (key, _) in _pool)
            Console.WriteLine($"     • {key}");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Context — Extrinsic (Benzersiz) veri + Flyweight ref
// ═══════════════════════════════════════════════════════════

public class Transaction
{
    // ✅ Extrinsic state: her işlem için benzersiz
    public string   TransactionId { get; }
    public decimal  Amount        { get; }
    public DateTime Timestamp     { get; }
    public string   CardLastFour  { get; }

    // ✅ Flyweight referansı: sadece bir pointer (8 byte)
    //    Gerçek veri paylaşılan havuzda
    private readonly TransactionType _type;

    public Transaction(
        string transactionId,
        decimal amount,
        string cardLastFour,
        TransactionType type)
    {
        TransactionId = transactionId;
        Amount        = amount;
        Timestamp     = DateTime.UtcNow;
        CardLastFour  = cardLastFour;
        _type         = type;
    }

    public decimal Fee         => _type.CalculateFee(Amount);
    public decimal NetAmount   => Amount - Fee;
    public string  Currency    => _type.Currency;
    public string  Processor   => _type.ProcessorName;
    public string  Category    => _type.MerchantCategory;

    public void Display()
    {
        Console.WriteLine($"   [{TransactionId}] {Amount:C} {_type.Currency}" +
                          $" | {_type.ProcessorName}/{_type.MerchantCategory}" +
                          $" | Komisyon: {Fee:C} | Net: {NetAmount:C}");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Transaction Ledger (Context nesneleri yöneten istemci)
// ═══════════════════════════════════════════════════════════

public class TransactionLedger
{
    private readonly List<Transaction>       _transactions = new();
    private readonly TransactionTypeFactory  _factory      = new();

    public void Add(string txId, decimal amount, string card,
                    string currency, string category, string processor,
                    decimal interchangeFee = 0.015m, decimal schemesFee = 0.003m)
    {
        // Factory'den flyweight al (var ise cache'den, yoksa yeni oluştur)
        var type = _factory.GetOrCreate(currency, category, processor, interchangeFee, schemesFee);
        _transactions.Add(new Transaction(txId, amount, card, type));
    }

    public void PrintAll()
    {
        Console.WriteLine($"\n   Toplam {_transactions.Count} işlem:");
        foreach (var tx in _transactions)
            tx.Display();
    }

    public void PrintMemoryReport(int transactionCount)
    {
        // Her Transaction: ~60 byte (3 string + decimal + DateTime + 1 pointer)
        // Her TransactionType: ~200 byte (büyük logo URL dahil)
        long withoutFlyweight = transactionCount * (60 + 200);
        long withFlyweight    = (transactionCount * 60L) + (_factory.PoolSize * 200);
        long saved            = withoutFlyweight - withFlyweight;

        Console.WriteLine($"\n   ─── Bellek Tasarrufu Analizi ({transactionCount:N0} işlem) ───");
        Console.WriteLine($"   Pattern olmadan : ~{withoutFlyweight / 1024 / 1024:N0} MB");
        Console.WriteLine($"   Flyweight ile   : ~{withFlyweight    / 1024 / 1024:N0} MB");
        Console.WriteLine($"   Tasarruf        : ~{saved            / 1024 / 1024:N0} MB");
        Console.WriteLine($"   Flyweight pool  : {_factory.PoolSize} benzersiz tip");
        _factory.PrintPool();
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class FlyweightPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     ✅ FLYWEIGHT PATTERN — Ödeme İşlem Kayıtları        ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        var ledger = new TransactionLedger();

        Console.WriteLine("\n─── İşlemler Kaydediliyor ───\n");

        // Farklı kombinasyonlarda işlemler ekle
        // Factory sadece yeni kombinasyon için nesne oluşturur
        ledger.Add("TXN-001", 250.00m, "4242", "TRY", "FOOD",  "Visa");
        ledger.Add("TXN-002", 89.90m,  "5555", "TRY", "FOOD",  "Visa");       // ♻️ Cache
        ledger.Add("TXN-003", 1200.0m, "1234", "TRY", "TECH",  "Mastercard");
        ledger.Add("TXN-004", 45.00m,  "9876", "TRY", "FUEL",  "Troy");
        ledger.Add("TXN-005", 320.50m, "4242", "TRY", "TECH",  "Mastercard"); // ♻️ Cache
        ledger.Add("TXN-006", 75.00m,  "5555", "USD", "FOOD",  "Visa");
        ledger.Add("TXN-007", 510.00m, "1234", "EUR", "TECH",  "Mastercard");
        ledger.Add("TXN-008", 19.99m,  "9876", "TRY", "FOOD",  "Visa");       // ♻️ Cache
        ledger.Add("TXN-009", 880.00m, "4242", "TRY", "FUEL",  "Troy");       // ♻️ Cache
        ledger.Add("TXN-010", 150.00m, "5555", "USD", "TECH",  "Mastercard"); // ♻️ Cache

        ledger.PrintAll();

        // ✅ 10 işlem ama sadece 6 benzersiz flyweight oluştu
        ledger.PrintMemoryReport(transactionCount: 1_000_000);

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ 1M işlem için tekrarlayan veri 1 kez saklandı");
        Console.WriteLine("✓ Transaction nesnesi küçük: sadece pointer taşıyor");
        Console.WriteLine("✓ Factory cache'i otomatik yönetiyor");
        Console.WriteLine("✓ İstemci kod flyweight'ı hiç bilmiyor\n");
    }
}
