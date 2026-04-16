namespace DesignPatterns.Structural.Facade.Correct;

/// <summary>
/// ✅ FACADE PATTERN: Karmaşık alt sistemi basit arayüzle gizle
///
/// Amaç:
/// - Karmaşık alt sistem kümesine basitleştirilmiş arayüz sağla
/// - İstemci kodunu alt sistemlerden izole et
/// - Alt sistemler değişirse istemci kodu etkilenmesin
/// - Ortak iş akışlarını tek noktada topla
///
/// Kaynak: https://refactoring.guru/design-patterns/facade
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Alt Sistemler (Complex Subsystems)
// Facade bunları koordine eder; istemci bunları bilmez
// ═══════════════════════════════════════════════════════════

public class InventoryService
{
    public bool CheckStock(int productId, int qty)
    {
        Console.WriteLine($"   📦 Stok kontrol: ürün={productId}, adet={qty}");
        return true;  // Demo: her zaman stokta var
    }

    public void ReserveStock(int productId, int qty)
        => Console.WriteLine($"   📦 Stok rezerve edildi: ürün={productId}, adet={qty}");

    public void ReleaseReservation(int productId, int qty)
        => Console.WriteLine($"   📦 Rezervasyon iptal: ürün={productId}, adet={qty}");

    public void ConfirmDeduction(int productId, int qty)
        => Console.WriteLine($"   📦 Stok düşüldü: ürün={productId}, adet={qty}");
}

public class PaymentService
{
    public record ChargeResult(bool Success, string TransactionId, decimal Amount);

    public ChargeResult Charge(string cardToken, decimal amount)
    {
        Console.WriteLine($"   💳 Ödeme alınıyor: {amount:C} (kart: {cardToken[..8]}…)");
        var txId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        Console.WriteLine($"   💳 Onaylandı: {txId}");
        return new ChargeResult(true, txId, amount);
    }

    public void Refund(string transactionId)
        => Console.WriteLine($"   💳 İade yapıldı: {transactionId}");
}

public class ShippingService
{
    public record ShippingLabel(string TrackingCode, string Carrier);

    public ShippingLabel CreateLabel(string userId, int productId, int qty)
    {
        var tracking = $"TR{new Random().Next(100000000, 999999999)}";
        Console.WriteLine($"   🚚 Kargo etiketi oluşturuldu: {tracking}");
        return new ShippingLabel(tracking, "Aras Kargo");
    }

    public void Dispatch(string trackingCode)
        => Console.WriteLine($"   🚚 Kargoya verildi: {trackingCode}");
}

public class InvoiceService
{
    public record Invoice(string InvoiceNo, decimal Amount);

    public Invoice Generate(string userId, int productId, int qty, decimal amount)
    {
        var no = $"INV-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        Console.WriteLine($"   🧾 Fatura oluşturuldu: {no} ({amount:C})");
        return new Invoice(no, amount);
    }

    public void Send(Invoice invoice, string userId)
        => Console.WriteLine($"   🧾 Fatura e-posta ile gönderildi: {invoice.InvoiceNo}");
}

public class NotificationService
{
    public void SendOrderConfirmation(string userId, string trackingCode)
        => Console.WriteLine($"   🔔 Sipariş onay bildirimi gönderildi → kullanıcı={userId}, takip={trackingCode}");

    public void SendCancellationNotice(string userId)
        => Console.WriteLine($"   🔔 İptal bildirimi gönderildi → kullanıcı={userId}");
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Facade (Alt sistemleri koordine eden basit arayüz)
// ═══════════════════════════════════════════════════════════

public record OrderResult(bool Success, string? TrackingCode, string? InvoiceNo, string? Error);

public class OrderFacade
{
    // ✅ Alt sistemler Facade'ın içinde — istemci görmez
    private readonly InventoryService  _inventory    = new();
    private readonly PaymentService    _payment      = new();
    private readonly ShippingService   _shipping     = new();
    private readonly InvoiceService    _invoice      = new();
    private readonly NotificationService _notification = new();

    /// <summary>
    /// ✅ İstemcinin bilmesi gereken tek metod.
    /// Sıra, rollback ve koordinasyon burada kapsüllenmiş.
    /// </summary>
    public OrderResult PlaceOrder(int productId, int qty, string userId, string cardToken, decimal price)
    {
        Console.WriteLine("\n   ─── Sipariş İşleme Başladı ───");

        // 1. Stok kontrolü
        if (!_inventory.CheckStock(productId, qty))
            return new OrderResult(false, null, null, "Stok yetersiz");

        _inventory.ReserveStock(productId, qty);

        // 2. Ödeme — başarısız olursa stok rezervasyonunu geri al
        var charge = _payment.Charge(cardToken, price * qty);
        if (!charge.Success)
        {
            _inventory.ReleaseReservation(productId, qty);
            return new OrderResult(false, null, null, "Ödeme başarısız");
        }

        // 3. Kargo
        var label = _shipping.CreateLabel(userId, productId, qty);
        _shipping.Dispatch(label.TrackingCode);

        // 4. Stok kesin düşümü
        _inventory.ConfirmDeduction(productId, qty);

        // 5. Fatura
        var invoice = _invoice.Generate(userId, productId, qty, charge.Amount);
        _invoice.Send(invoice, userId);

        // 6. Bildirim
        _notification.SendOrderConfirmation(userId, label.TrackingCode);

        Console.WriteLine("   ─── Sipariş Tamamlandı ───");
        return new OrderResult(true, label.TrackingCode, invoice.InvoiceNo, null);
    }

    /// <summary>
    /// ✅ İptal akışı da Facade'da kapsüllenmiş
    /// </summary>
    public OrderResult CancelOrder(string transactionId, int productId, int qty, string userId)
    {
        Console.WriteLine("\n   ─── Sipariş İptali Başladı ───");

        _payment.Refund(transactionId);
        _inventory.ReleaseReservation(productId, qty);
        _notification.SendCancellationNotice(userId);

        Console.WriteLine("   ─── İptal Tamamlandı ───");
        return new OrderResult(true, null, null, null);
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class FacadePatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        ✅ FACADE PATTERN — Sipariş İşleme Sistemi       ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        // ✅ İstemci sadece Facade biliyor
        var orderFacade = new OrderFacade();

        // Web API controller
        Console.WriteLine("\n─── Web API: Sipariş Ver ───");
        var result = orderFacade.PlaceOrder(
            productId: 42,
            qty:       2,
            userId:    "user-001",
            cardToken: "tok_visa_4242424242",
            price:     149.99m);

        if (result.Success)
        {
            Console.WriteLine($"\n   ✅ Sipariş başarılı!");
            Console.WriteLine($"      Takip: {result.TrackingCode}");
            Console.WriteLine($"      Fatura: {result.InvoiceNo}");
        }

        // Mobil API — aynı Facade, aynı kolaylık
        Console.WriteLine("\n─── Mobil API: Başka Sipariş ───");
        var result2 = orderFacade.PlaceOrder(
            productId: 17,
            qty:       1,
            userId:    "user-002",
            cardToken: "tok_mc_5555555555",
            price:     299.00m);

        Console.WriteLine($"\n   ✅ Sipariş başarılı! Takip: {result2.TrackingCode}");

        // İptal
        Console.WriteLine("\n─── Sipariş İptali ───");
        orderFacade.CancelOrder("TXN-ABCD1234", productId: 42, qty: 2, userId: "user-001");

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ Controller sadece OrderFacade.PlaceOrder() biliyor");
        Console.WriteLine("✓ 5 alt servis istemciden tamamen gizlenmiş");
        Console.WriteLine("✓ Rollback mantığı Facade'da — istemci yazmıyor");
        Console.WriteLine("✓ Alt servis değişse sadece Facade güncellenir\n");
    }
}
