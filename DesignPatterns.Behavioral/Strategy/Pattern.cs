namespace DesignPatterns.Behavioral.Strategy.Correct;

/// <summary>
/// ✅ STRATEGY PATTERN: Algoritmayı kapsülle, runtime'da değiştir
///
/// Amaç:
/// - Birbirine alternatif algoritmaları bir aile olarak tanımla
/// - Her algoritmayı ayrı sınıfa kapsülle
/// - Runtime'da algoritmayı değiştirilebilir yap
/// - Context'i algoritmadan bağımsız hale getir
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Strategy Interface
// ═══════════════════════════════════════════════════════════

public interface IPaymentStrategy
{
    string MethodName { get; }
    bool Validate();
    void Pay(decimal amount);
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Concrete Strategies (Her algoritma kendi sınıfında)
// ═══════════════════════════════════════════════════════════

public class CreditCardPayment : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _cvv;
    private readonly string _expiryDate;

    public string MethodName => "Kredi Kartı";

    public CreditCardPayment(string cardNumber, string cvv, string expiryDate)
    {
        _cardNumber = cardNumber;
        _cvv = cvv;
        _expiryDate = expiryDate;
    }

    public bool Validate()
    {
        Console.WriteLine("   Kart numarası doğrulanıyor...");
        Console.WriteLine("   CVV kontrol ediliyor...");
        Console.WriteLine("   Son kullanma tarihi kontrol ediliyor...");
        return _cardNumber.Length == 16 && _cvv.Length == 3;
    }

    public void Pay(decimal amount)
    {
        Console.WriteLine($"   ✓ {amount:C} kredi kartından çekildi.");
        Console.WriteLine($"   Kart: **** **** **** {_cardNumber[^4..]}");
    }
}

public class PayPalPayment : IPaymentStrategy
{
    private readonly string _email;
    private readonly string _password;

    public string MethodName => "PayPal";

    public PayPalPayment(string email, string password)
    {
        _email = email;
        _password = password;
    }

    public bool Validate()
    {
        Console.WriteLine("   PayPal hesabı doğrulanıyor...");
        Console.WriteLine($"   E-posta: {_email}");
        return _email.Contains('@') && _password.Length >= 6;
    }

    public void Pay(decimal amount)
    {
        Console.WriteLine($"   ✓ {amount:C} PayPal hesabından çekildi.");
    }
}

public class CryptoPayment : IPaymentStrategy
{
    private readonly string _walletAddress;
    private readonly string _currency;

    public string MethodName => $"Kripto ({_currency})";

    public CryptoPayment(string walletAddress, string currency = "BTC")
    {
        _walletAddress = walletAddress;
        _currency = currency;
    }

    public bool Validate()
    {
        Console.WriteLine("   Cüzdan adresi kontrol ediliyor...");
        Console.WriteLine("   Blockchain bağlantısı doğrulanıyor...");
        return _walletAddress.Length >= 26;
    }

    public void Pay(decimal amount)
    {
        Console.WriteLine($"   ✓ {amount:C} {_currency} cüzdanından gönderildi.");
        Console.WriteLine($"   Cüzdan: {_walletAddress[..8]}...{_walletAddress[^4..]}");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Context (Strategy'yi kullanır, detayları bilmez)
// ═══════════════════════════════════════════════════════════

public class PaymentService
{
    private IPaymentStrategy? _strategy;

    // ✅ Runtime'da strateji seçimi / değiştirme
    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy;
        Console.WriteLine($"\n▶ Ödeme yöntemi seçildi: {strategy.MethodName}");
    }

    public bool ProcessPayment(decimal amount)
    {
        if (_strategy is null)
        {
            Console.WriteLine("❌ Ödeme yöntemi seçilmedi!");
            return false;
        }

        Console.WriteLine($"\n--- Ödeme İşlemi ({_strategy.MethodName}) ---");

        if (!_strategy.Validate())
        {
            Console.WriteLine("❌ Doğrulama başarısız, ödeme iptal.");
            return false;
        }

        _strategy.Pay(amount);
        return true;
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class StrategyPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         ✅ STRATEGY PATTERN — Ödeme Sistemi             ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        var paymentService = new PaymentService();

        // Kredi kartı
        paymentService.SetStrategy(new CreditCardPayment("4242424242424242", "123", "12/26"));
        paymentService.ProcessPayment(299.99m);

        // Runtime'da PayPal'a geç
        paymentService.SetStrategy(new PayPalPayment("user@example.com", "securepass"));
        paymentService.ProcessPayment(149.00m);

        // Runtime'da Kripto'ya geç
        paymentService.SetStrategy(new CryptoPayment("1A2B3C4D5E6F7G8H9I0J1K2L3M4N5O6P", "ETH"));
        paymentService.ProcessPayment(89.50m);

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ Yeni ödeme yöntemi = yeni sınıf, PaymentService değişmez");
        Console.WriteLine("✓ Her algoritma izole, test edilebilir");
        Console.WriteLine("✓ Runtime'da strateji değiştirme");
        Console.WriteLine("✓ Open/Closed Principle ✅\n");
    }
}
