namespace DesignPatterns.Structural.Bridge.Correct;

/// <summary>
/// ✅ BRIDGE PATTERN: İki bağımsız boyutu ayır, composition ile bağla
///
/// Amaç:
/// - Abstraction (ödeme türü) ve Implementation (ödeme kanalı) ayrı hiyerarşi
/// - Her boyut bağımsız olarak genişleyebilsin
/// - N×M alt sınıf yerine N+M sınıf yeterli olsun
/// - Runtime'da implementation değiştirilebilsin
///
/// Kaynak: https://refactoring.guru/design-patterns/bridge
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Implementation Interface — BOYUT 2 (Ödeme Kanalı)
// Abstraction bunları kullanır ama somut sınıflarını bilmez
// ═══════════════════════════════════════════════════════════

public record ChannelResult(bool Success, string Reference, string? Error = null);

public interface IPaymentChannel
{
    string ChannelName { get; }
    ChannelResult Collect(decimal amount, string description);
    ChannelResult Refund(string reference, decimal amount);
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Concrete Implementations — Kanallar
// ═══════════════════════════════════════════════════════════

public class CreditCardChannel : IPaymentChannel
{
    private readonly string _cardToken;
    private readonly string _cvv;

    public string ChannelName => "Kredi Kartı";

    public CreditCardChannel(string cardToken, string cvv)
    {
        _cardToken = cardToken;
        _cvv       = cvv;
    }

    public ChannelResult Collect(decimal amount, string description)
    {
        Console.WriteLine($"   💳 Kredi Kartı tahsilat: {amount:C}");
        Console.WriteLine($"      Kart: **** {_cardToken[^4..]}  Açıklama: {description}");
        var ref_ = $"CC-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        Console.WriteLine($"      Referans: {ref_}");
        return new ChannelResult(true, ref_);
    }

    public ChannelResult Refund(string reference, decimal amount)
    {
        Console.WriteLine($"   💳 Kredi Kartı iade: {amount:C}  (ref: {reference})");
        return new ChannelResult(true, $"REF-{reference}");
    }
}

public class BankTransferChannel : IPaymentChannel
{
    private readonly string _iban;

    public string ChannelName => "Banka Transferi";

    public BankTransferChannel(string iban)
        => _iban = iban;

    public ChannelResult Collect(decimal amount, string description)
    {
        Console.WriteLine($"   🏦 Banka Transferi tahsilat: {amount:C}");
        Console.WriteLine($"      IBAN: {_iban[..4]}****{_iban[^4..]}  Açıklama: {description}");
        var ref_ = $"BT-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        Console.WriteLine($"      Referans: {ref_}");
        return new ChannelResult(true, ref_);
    }

    public ChannelResult Refund(string reference, decimal amount)
    {
        Console.WriteLine($"   🏦 Banka Transferi iade: {amount:C}  (ref: {reference})");
        return new ChannelResult(true, $"REF-{reference}");
    }
}

public class DigitalWalletChannel : IPaymentChannel
{
    private readonly string _walletId;
    private readonly string _provider;

    public string ChannelName => $"Dijital Cüzdan ({_provider})";

    public DigitalWalletChannel(string walletId, string provider = "Papara")
    {
        _walletId = walletId;
        _provider = provider;
    }

    public ChannelResult Collect(decimal amount, string description)
    {
        Console.WriteLine($"   👛 {_provider} tahsilat: {amount:C}");
        Console.WriteLine($"      Cüzdan: {_walletId}  Açıklama: {description}");
        var ref_ = $"DW-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        Console.WriteLine($"      Referans: {ref_}");
        return new ChannelResult(true, ref_);
    }

    public ChannelResult Refund(string reference, decimal amount)
    {
        Console.WriteLine($"   👛 {_provider} iade: {amount:C}  (ref: {reference})");
        return new ChannelResult(true, $"REF-{reference}");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Abstraction — BOYUT 1 (Ödeme Türü)
// ✅ IPaymentChannel referansı taşır (bridge/köprü)
// ═══════════════════════════════════════════════════════════

public abstract class Payment
{
    // ✅ Bridge: Abstraction, Implementation'ı composition ile tutar
    protected readonly IPaymentChannel _channel;

    protected Payment(IPaymentChannel channel)
        => _channel = channel;

    public abstract bool Process(decimal amount);

    // Runtime'da kanalı değiştir
    public virtual Payment WithChannel(IPaymentChannel newChannel)
        => (Payment)Activator.CreateInstance(GetType(), newChannel)!;
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Refined Abstractions — Ödeme Türleri
// Her tür kanalın detayını bilmez, sadece _channel.Collect() çağırır
// ═══════════════════════════════════════════════════════════

public class OneTimePayment : Payment
{
    public OneTimePayment(IPaymentChannel channel) : base(channel) { }

    public override bool Process(decimal amount)
    {
        Console.WriteLine($"\n   ── Tek Seferlik Ödeme | Kanal: {_channel.ChannelName} ──");
        var result = _channel.Collect(amount, "Tek seferlik ödeme");
        Console.WriteLine($"   {(result.Success ? "✅" : "❌")} Sonuç: {result.Reference}");
        return result.Success;
    }
}

public class InstallmentPayment : Payment
{
    private readonly int _installments;

    public InstallmentPayment(IPaymentChannel channel, int installments = 3)
        : base(channel) => _installments = installments;

    public override bool Process(decimal amount)
    {
        var perInstallment = Math.Round(amount / _installments, 2);
        Console.WriteLine($"\n   ── Taksitli Ödeme ({_installments}×{perInstallment:C}) | Kanal: {_channel.ChannelName} ──");

        for (int i = 1; i <= _installments; i++)
        {
            Console.WriteLine($"   Taksit {i}/{_installments}:");
            var result = _channel.Collect(perInstallment, $"Taksit {i}/{_installments}");
            if (!result.Success)
            {
                Console.WriteLine($"   ❌ Taksit {i} başarısız: {result.Error}");
                return false;
            }
        }

        Console.WriteLine($"   ✅ Tüm taksitler tamamlandı");
        return true;
    }
}

public class RecurringPayment : Payment
{
    private readonly string _interval;
    private string? _subscriptionRef;

    public RecurringPayment(IPaymentChannel channel, string interval = "monthly")
        : base(channel) => _interval = interval;

    public override bool Process(decimal amount)
    {
        Console.WriteLine($"\n   ── Abonelik Ödemesi ({_interval}) | Kanal: {_channel.ChannelName} ──");
        var result = _channel.Collect(amount, $"Abonelik - {_interval}");

        if (result.Success)
        {
            _subscriptionRef = result.Reference;
            Console.WriteLine($"   ✅ Abonelik aktif: {_subscriptionRef}");
        }

        return result.Success;
    }

    public bool Cancel()
    {
        if (_subscriptionRef is null) return false;
        Console.WriteLine($"   🔴 Abonelik iptal: {_subscriptionRef}");
        return true;
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class BridgePatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     ✅ BRIDGE PATTERN — Ödeme Türü × Ödeme Kanalı      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        // Kanallar (Implementation hiyerarşisi)
        IPaymentChannel card     = new CreditCardChannel("4242424242424242", "123");
        IPaymentChannel transfer = new BankTransferChannel("TR330006100519786457841326");
        IPaymentChannel wallet   = new DigitalWalletChannel("papara-user-001", "Papara");

        Console.WriteLine("\n─── Aynı TÜR, farklı KANAL ───");
        new OneTimePayment(card).Process(500m);
        new OneTimePayment(transfer).Process(500m);
        new OneTimePayment(wallet).Process(500m);

        Console.WriteLine("\n─── Aynı KANAL, farklı TÜR ───");
        new OneTimePayment(card).Process(1_200m);
        new InstallmentPayment(card, installments: 3).Process(1_200m);
        new RecurringPayment(card, interval: "monthly").Process(1_200m);

        // ✅ Runtime'da kanal değiştirme
        Console.WriteLine("\n─── Runtime'da Kanal Değiştirme ───");
        Payment payment = new InstallmentPayment(card, installments: 6);
        payment.Process(3_000m);

        Console.WriteLine("\n   Kanal değişti → Banka Transferi:");
        Payment sameTurkWithNewChannel = new InstallmentPayment(transfer, installments: 6);
        sameTurkWithNewChannel.Process(3_000m);

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine($"✓ 3 tür + 3 kanal = 6 sınıf (kalıtımda 3×3=9 olurdu)");
        Console.WriteLine("✓ Yeni kanal (kripto) = sadece 1 yeni sınıf, türler değişmez");
        Console.WriteLine("✓ Yeni tür (ertelenmiş) = sadece 1 yeni sınıf, kanallar değişmez");
        Console.WriteLine("✓ Runtime'da kanal takası mümkün\n");
    }
}
