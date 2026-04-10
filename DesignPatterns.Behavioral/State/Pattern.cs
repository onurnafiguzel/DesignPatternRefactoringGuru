namespace DesignPatterns.Behavioral.State.Correct;

/// <summary>
/// ✅ STATE PATTERN: Durumu kapsülle, davranışı duruma göre değiştir
///
/// Amaç:
/// - Nesnenin iç durumu değiştikçe davranışı da değişsin
/// - Her durumu ayrı sınıfa kapsülle
/// - Durum geçiş kurallarını merkezi if/else'ten kurtarır
/// - Yeni durum eklemek için mevcut kodu değiştirmek gerekmez
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: State Interface
// ═══════════════════════════════════════════════════════════

public interface IOrderState
{
    string Name { get; }
    void Cancel(Order order);
    void Ship(Order order);
    void Deliver(Order order);
    void Refund(Order order);
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Concrete States
// ═══════════════════════════════════════════════════════════

public class PendingState : IOrderState
{
    public string Name => "Beklemede";

    public void Cancel(Order order)
    {
        Console.WriteLine("   ✅ Sipariş iptal edildi.");
        order.SetState(new CancelledState());
    }

    public void Ship(Order order)
    {
        Console.WriteLine("   ✅ Sipariş kargoya verildi.");
        order.SetState(new ShippedState());
    }

    public void Deliver(Order order) =>
        Console.WriteLine("   ❌ Henüz kargoya verilmedi, teslim edilemez.");

    public void Refund(Order order) =>
        Console.WriteLine("   ❌ Teslim edilmemiş sipariş iade edilemez.");
}

public class ShippedState : IOrderState
{
    public string Name => "Kargoda";

    public void Cancel(Order order) =>
        Console.WriteLine("   ❌ Kargodaki sipariş iptal edilemez.");

    public void Ship(Order order) =>
        Console.WriteLine("   ❌ Sipariş zaten kargoda.");

    public void Deliver(Order order)
    {
        Console.WriteLine("   ✅ Sipariş teslim edildi.");
        order.SetState(new DeliveredState());
    }

    public void Refund(Order order) =>
        Console.WriteLine("   ❌ Teslim edilmeden iade yapılamaz.");
}

public class DeliveredState : IOrderState
{
    public string Name => "Teslim Edildi";

    public void Cancel(Order order) =>
        Console.WriteLine("   ❌ Teslim edilmiş sipariş iptal edilemez.");

    public void Ship(Order order) =>
        Console.WriteLine("   ❌ Sipariş zaten teslim edildi.");

    public void Deliver(Order order) =>
        Console.WriteLine("   ❌ Sipariş zaten teslim edildi.");

    public void Refund(Order order)
    {
        Console.WriteLine("   ✅ İade talebi oluşturuldu.");
        order.SetState(new RefundedState());
    }
}

public class CancelledState : IOrderState
{
    public string Name => "İptal Edildi";

    public void Cancel(Order order) =>
        Console.WriteLine("   ❌ Sipariş zaten iptal edildi.");

    public void Ship(Order order) =>
        Console.WriteLine("   ❌ İptal edilmiş sipariş kargoya verilemez.");

    public void Deliver(Order order) =>
        Console.WriteLine("   ❌ İptal edilmiş sipariş teslim edilemez.");

    public void Refund(Order order) =>
        Console.WriteLine("   ❌ İptal edilmiş siparişte iade olmaz.");
}

public class RefundedState : IOrderState
{
    public string Name => "İade Edildi";

    public void Cancel(Order order)  => Console.WriteLine("   ❌ İade edilmiş sipariş üzerinde işlem yapılamaz.");
    public void Ship(Order order)    => Console.WriteLine("   ❌ İade edilmiş sipariş üzerinde işlem yapılamaz.");
    public void Deliver(Order order) => Console.WriteLine("   ❌ İade edilmiş sipariş üzerinde işlem yapılamaz.");
    public void Refund(Order order)  => Console.WriteLine("   ❌ Sipariş zaten iade edildi.");
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Context (State'i tutar, işlemleri state'e devreder)
// ═══════════════════════════════════════════════════════════

public class Order
{
    private IOrderState _state;
    public string OrderId { get; }

    public Order(string orderId)
    {
        OrderId = orderId;
        _state = new PendingState();
        Console.WriteLine($"📦 Sipariş oluşturuldu: {orderId} → [{_state.Name}]");
    }

    // ✅ Sadece state sınıfları SetState'i çağırır (internal geçiş)
    public void SetState(IOrderState state)
    {
        Console.WriteLine($"   🔄 Durum: [{_state.Name}] → [{state.Name}]");
        _state = state;
    }

    // ✅ Order metotları if/else içermez, tamamen state'e delege eder
    public void Cancel()  { Console.WriteLine($"\n▶ {OrderId} — İptal işlemi:"); _state.Cancel(this); }
    public void Ship()    { Console.WriteLine($"\n▶ {OrderId} — Kargo işlemi:"); _state.Ship(this); }
    public void Deliver() { Console.WriteLine($"\n▶ {OrderId} — Teslim işlemi:"); _state.Deliver(this); }
    public void Refund()  { Console.WriteLine($"\n▶ {OrderId} — İade işlemi:"); _state.Refund(this); }

    public string CurrentState => _state.Name;
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class StatePatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         ✅ STATE PATTERN — Sipariş Yönetimi             ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // Mutlu yol: Pending → Shipped → Delivered → Refunded
        Console.WriteLine("─── Senaryo 1: Normal sipariş akışı ───");
        var order1 = new Order("ORD-001");
        order1.Ship();
        order1.Deliver();
        order1.Refund();

        // Geçersiz geçişler
        Console.WriteLine("\n─── Senaryo 2: Geçersiz işlemler ───");
        var order2 = new Order("ORD-002");
        order2.Deliver();  // ❌ Henüz kargoda değil
        order2.Ship();     // ✅ Kargoya ver
        order2.Cancel();   // ❌ Kargodayken iptal olmaz
        order2.Deliver();  // ✅ Teslim et

        // Pending'den iptal
        Console.WriteLine("\n─── Senaryo 3: Erken iptal ───");
        var order3 = new Order("ORD-003");
        order3.Cancel();   // ✅ Beklemedeyken iptal olur
        order3.Ship();     // ❌ İptal sonrası işlem yapılamaz

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ Her durum kendi kurallarını biliyor");
        Console.WriteLine("✓ Order.Ship() içinde if/else yok, state'e delege");
        Console.WriteLine("✓ Yeni durum = yeni sınıf, Order değişmez");
        Console.WriteLine("✓ Geçiş kuralları nerede? İlgili State sınıfında ✅\n");
    }
}
