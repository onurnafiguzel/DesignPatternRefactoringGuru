namespace DesignPatterns.Structural.Proxy.Correct;

/// <summary>
/// ✅ PROXY PATTERN: Gerçek nesneye erişimi kontrol et
///
/// Amaç:
/// - Gerçek nesneyle aynı interface'i uygula
/// - Erişimi kontrol et, loglama, cache, rate limit ekle
/// - Gerçek nesneyi değiştirmeden araya gir
/// - İstemci proxy mi gerçek mi bilmeden çalışsın
///
/// Kaynak: https://refactoring.guru/design-patterns/proxy
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Subject Interface (Proxy ve gerçek aynı sözleşme)
// ═══════════════════════════════════════════════════════════

public record ChargeResult(bool Success, string TransactionId, decimal Amount, string? Error = null);
public record RefundResult(bool Success, string? Error = null);

public interface IPaymentGateway
{
    ChargeResult Charge(string cardToken, decimal amount, string userId);
    RefundResult Refund(string transactionId, string userId);
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Real Subject — Sadece ödeme alır, başka bilmez
// ═══════════════════════════════════════════════════════════

public class RealPaymentGateway : IPaymentGateway
{
    public ChargeResult Charge(string cardToken, decimal amount, string userId)
    {
        // Gerçekte: banka API çağrısı ~300ms
        Console.WriteLine($"   🏦 [BankAPI] Ödeme işleniyor: {amount:C}");
        var txId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        Console.WriteLine($"   🏦 [BankAPI] Onaylandı: {txId}");
        return new ChargeResult(true, txId, amount);
    }

    public RefundResult Refund(string transactionId, string userId)
    {
        Console.WriteLine($"   🏦 [BankAPI] İade işleniyor: {transactionId}");
        return new RefundResult(true);
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Yardımcı servisler (Proxy'nin kullandığı)
// ═══════════════════════════════════════════════════════════

public class AuthService
{
    private static readonly HashSet<string> _authorizedUsers = ["user-001", "user-002", "admin-001"];

    public bool IsAuthorized(string userId)
    {
        var result = _authorizedUsers.Contains(userId);
        Console.WriteLine($"   🔑 Auth: {userId} → {(result ? "✅ Yetkili" : "❌ Yetkisiz")}");
        return result;
    }
}

public class FraudDetectionService
{
    private const decimal SuspiciousThreshold = 10_000m;

    public bool IsSuspicious(string userId, decimal amount)
    {
        var suspicious = amount > SuspiciousThreshold;
        if (suspicious)
            Console.WriteLine($"   🚨 Fraud: {userId} için {amount:C} şüpheli işlem!");
        else
            Console.WriteLine($"   🛡️  Fraud: {amount:C} normal aralıkta");
        return suspicious;
    }
}

public class RateLimiter
{
    private readonly Dictionary<string, (int Count, DateTime Window)> _counters = new();
    private const int MaxRequestsPerMinute = 5;

    public bool IsAllowed(string userId)
    {
        var now = DateTime.UtcNow;
        if (!_counters.TryGetValue(userId, out var entry) ||
            (now - entry.Window).TotalMinutes >= 1)
        {
            _counters[userId] = (1, now);
            Console.WriteLine($"   🚦 RateLimit: {userId} → 1/{MaxRequestsPerMinute}");
            return true;
        }

        if (entry.Count >= MaxRequestsPerMinute)
        {
            Console.WriteLine($"   🚦 RateLimit: {userId} → LIMIT AŞILDI ({entry.Count}/{MaxRequestsPerMinute})");
            return false;
        }

        _counters[userId] = (entry.Count + 1, entry.Window);
        Console.WriteLine($"   🚦 RateLimit: {userId} → {entry.Count + 1}/{MaxRequestsPerMinute}");
        return true;
    }
}

public class PaymentLogger
{
    public void LogAttempt(string userId, decimal amount)
        => Console.WriteLine($"   📋 LOG [{DateTime.UtcNow:HH:mm:ss}] ATTEMPT | user={userId} amount={amount:C}");

    public void LogSuccess(string userId, string txId, decimal amount)
        => Console.WriteLine($"   📋 LOG [{DateTime.UtcNow:HH:mm:ss}] SUCCESS | user={userId} tx={txId} amount={amount:C}");

    public void LogFailure(string userId, string reason)
        => Console.WriteLine($"   📋 LOG [{DateTime.UtcNow:HH:mm:ss}] FAILURE | user={userId} reason={reason}");
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Protection Proxy — Gerçeğin önünde tüm kontroller
// ═══════════════════════════════════════════════════════════

public class PaymentGatewayProxy : IPaymentGateway
{
    private readonly IPaymentGateway       _real;      // Gerçek gateway
    private readonly AuthService           _auth       = new();
    private readonly FraudDetectionService _fraud      = new();
    private readonly RateLimiter           _rateLimiter = new();
    private readonly PaymentLogger         _logger     = new();

    public PaymentGatewayProxy(IPaymentGateway realGateway)
        => _real = realGateway;

    public ChargeResult Charge(string cardToken, decimal amount, string userId)
    {
        Console.WriteLine($"\n   ── Proxy kontrolleri: Charge({amount:C}, user={userId}) ──");

        // 1. Yetki kontrolü
        if (!_auth.IsAuthorized(userId))
        {
            _logger.LogFailure(userId, "Unauthorized");
            return new ChargeResult(false, "", 0, "Yetkisiz erişim");
        }

        // 2. Rate limiting
        if (!_rateLimiter.IsAllowed(userId))
        {
            _logger.LogFailure(userId, "RateLimitExceeded");
            return new ChargeResult(false, "", 0, "İstek limiti aşıldı");
        }

        // 3. Fraud tespiti
        if (_fraud.IsSuspicious(userId, amount))
        {
            _logger.LogFailure(userId, "FraudSuspected");
            return new ChargeResult(false, "", 0, "Şüpheli işlem engellendi");
        }

        // 4. Log: başlamadan önce
        _logger.LogAttempt(userId, amount);

        // 5. ✅ Tüm kontroller geçti — gerçek gateway'e ilet
        var result = _real.Charge(cardToken, amount, userId);

        // 6. Log: sonuç
        if (result.Success)
            _logger.LogSuccess(userId, result.TransactionId, result.Amount);
        else
            _logger.LogFailure(userId, result.Error ?? "Unknown");

        return result;
    }

    public RefundResult Refund(string transactionId, string userId)
    {
        Console.WriteLine($"\n   ── Proxy kontrolleri: Refund(tx={transactionId}) ──");

        if (!_auth.IsAuthorized(userId))
        {
            _logger.LogFailure(userId, "Unauthorized");
            return new RefundResult(false, "Yetkisiz erişim");
        }

        Console.WriteLine($"   📋 LOG İade başlatıldı: {transactionId}");
        return _real.Refund(transactionId, userId);
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class ProxyPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║      ✅ PROXY PATTERN — Ödeme Gateway Koruması          ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        // ✅ İstemci proxy mi gerçek mi bilmiyor — IPaymentGateway
        IPaymentGateway gateway = new PaymentGatewayProxy(new RealPaymentGateway());

        // Senaryo 1: Normal ödeme
        Console.WriteLine("\n─── Senaryo 1: Geçerli Ödeme ───");
        var r1 = gateway.Charge("tok_visa_4242", 250m, "user-001");
        Console.WriteLine($"   Sonuç: {(r1.Success ? $"✅ {r1.TransactionId}" : $"❌ {r1.Error}")}");

        // Senaryo 2: Yetkisiz kullanıcı
        Console.WriteLine("\n─── Senaryo 2: Yetkisiz Kullanıcı ───");
        var r2 = gateway.Charge("tok_mc_5555", 100m, "hacker-999");
        Console.WriteLine($"   Sonuç: {(r2.Success ? "✅" : $"❌ {r2.Error}")}");

        // Senaryo 3: Şüpheli tutar (fraud)
        Console.WriteLine("\n─── Senaryo 3: Fraud Tespiti ───");
        var r3 = gateway.Charge("tok_visa_4242", 15_000m, "user-002");
        Console.WriteLine($"   Sonuç: {(r3.Success ? "✅" : $"❌ {r3.Error}")}");

        // Senaryo 4: Rate limit (aynı kullanıcı art arda)
        Console.WriteLine("\n─── Senaryo 4: Rate Limit ───");
        for (int i = 1; i <= 7; i++)
        {
            var r = gateway.Charge("tok_visa_4242", 50m, "user-001");
            Console.WriteLine($"   İstek {i}: {(r.Success ? $"✅ {r.TransactionId}" : $"❌ {r.Error}")}");
        }

        // Senaryo 5: İade
        Console.WriteLine("\n─── Senaryo 5: İade ───");
        var refund = gateway.Refund("TXN-ABCD1234", "user-001");
        Console.WriteLine($"   Sonuç: {(refund.Success ? "✅ İade tamamlandı" : $"❌ {refund.Error}")}");

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ RealPaymentGateway sadece banka API'si biliyor");
        Console.WriteLine("✓ Auth, fraud, rate limit, log → Proxy'de merkezi");
        Console.WriteLine("✓ İstemci IPaymentGateway görüyor, proxy mi gerçek mi bilmiyor");
        Console.WriteLine("✓ Yeni kural = Proxy değişir, RealGateway dokunulmaz\n");
    }
}
