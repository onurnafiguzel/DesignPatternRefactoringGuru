namespace DesignPatterns.Structural.Composite.Correct;

/// <summary>
/// ✅ COMPOSITE PATTERN: Tekil ve grup nesneleri aynı interface ile kullan
///
/// Amaç:
/// - Nesneleri ağaç yapısında düzenle
/// - Tek nesne (Leaf) ve grup (Composite) aynı interface'i uygulasın
/// - İstemci kod tekil mi grup mu bilmeden çalışsın
/// - Recursive işlemler doğal olarak çalışsın
///
/// Kaynak: https://refactoring.guru/design-patterns/composite
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Component Interface (Leaf ve Composite aynı sözleşme)
// ═══════════════════════════════════════════════════════════

public interface IPaymentComponent
{
    string Name { get; }
    decimal Calculate();     // Toplam tutarı hesapla (recursive)
    void Cancel();           // İptal et (recursive)
    void Display(int indent = 0);  // Ağaç görünümü
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Leaf — Tekil Ödeme (Alt bileşeni olmayan yaprak)
// ═══════════════════════════════════════════════════════════

public class SinglePayment : IPaymentComponent
{
    public string  Name      { get; }
    public decimal Amount    { get; }
    public string  Currency  { get; }
    private bool   _cancelled;

    public SinglePayment(string name, decimal amount, string currency = "TRY")
    {
        Name     = name;
        Amount   = amount;
        Currency = currency;
    }

    // ✅ Yaprak: doğrudan tutarı döner, recursive değil
    public decimal Calculate() => _cancelled ? 0 : Amount;

    public void Cancel()
    {
        _cancelled = true;
        Console.WriteLine($"   🔴 İptal edildi: {Name} ({Amount:C})");
    }

    public void Display(int indent = 0)
    {
        var prefix = new string(' ', indent * 3);
        var status = _cancelled ? " [İPTAL]" : "";
        Console.WriteLine($"{prefix}💳 {Name}: {Amount:C}{status}");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Composite — Ödeme Paketi (Alt bileşenleri olan dal)
// ═══════════════════════════════════════════════════════════

public class PaymentBundle : IPaymentComponent
{
    public string Name { get; }
    private readonly List<IPaymentComponent> _children = new();
    private decimal? _discountAmount;

    public PaymentBundle(string name, decimal? discountAmount = null)
    {
        Name            = name;
        _discountAmount = discountAmount;
    }

    // ✅ Alt bileşen ekle / çıkar
    public PaymentBundle Add(IPaymentComponent component)
    {
        _children.Add(component);
        return this;  // Fluent API
    }

    public void Remove(IPaymentComponent component)
        => _children.Remove(component);

    // ✅ Composite: alt bileşenlere delege eder — tekil mi paket mi bilmez
    public decimal Calculate()
    {
        var subtotal = _children.Sum(c => c.Calculate());
        var discount = _discountAmount ?? 0;
        return Math.Max(0, subtotal - discount);
    }

    // ✅ Recursive iptal — tüm ağacı dolaşır
    public void Cancel()
    {
        Console.WriteLine($"   📦 Paket iptal ediliyor: {Name}");
        foreach (var child in _children)
            child.Cancel();
    }

    // ✅ Recursive görüntüleme — derinlik ne olursa
    public void Display(int indent = 0)
    {
        var prefix   = new string(' ', indent * 3);
        var subtotal = _children.Sum(c => c.Calculate());

        Console.WriteLine($"{prefix}📦 {Name}");

        foreach (var child in _children)
            child.Display(indent + 1);

        if (_discountAmount.HasValue)
            Console.WriteLine($"{prefix}   🏷️  İndirim: -{_discountAmount:C}");

        Console.WriteLine($"{prefix}   ──────────────────");
        Console.WriteLine($"{prefix}   Toplam: {Calculate():C}");
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class CompositePatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     ✅ COMPOSITE PATTERN — Ödeme Planı Ağacı            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        // ─── Yapraklar (Leaf) ───
        var temelPlan     = new SinglePayment("Temel Plan",           299m);
        var destekPaketi  = new SinglePayment("Destek Paketi",         99m);
        var kurulum       = new SinglePayment("Kurulum Ücreti",        149m);
        var egitim        = new SinglePayment("Online Eğitim",          79m);
        var yedekleme     = new SinglePayment("Bulut Yedekleme",        49m);
        var ozelRapor     = new SinglePayment("Özel Raporlama",        199m);

        // ─── İç içe paket ağacı ─────────────────────────────
        //
        // Kurumsal Paket (indirim: 100₺)
        // ├── Başlangıç Paketi
        // │   ├── Temel Plan         299₺
        // │   └── Destek Paketi       99₺
        // ├── Eklentiler
        // │   ├── Online Eğitim       79₺
        // │   └── Bulut Yedekleme     49₺
        // ├── Kurulum Ücreti         149₺
        // └── Özel Raporlama         199₺
        //                          ──────
        //                    İndirim: -100₺
        //                   TOPLAM: 773₺

        var baslangicPaketi = new PaymentBundle("Başlangıç Paketi")
            .Add(temelPlan)
            .Add(destekPaketi);

        var eklentiler = new PaymentBundle("Eklentiler")
            .Add(egitim)
            .Add(yedekleme);

        var kurumsal = new PaymentBundle("Kurumsal Paket", discountAmount: 100m)
            .Add(baslangicPaketi)   // ← Paket içinde paket
            .Add(eklentiler)        // ← Paket içinde paket
            .Add(kurulum)
            .Add(ozelRapor);

        // ─── Uniform işlem: istemci tekil mi paket mi bilmez ───
        Console.WriteLine("\n─── Fatura Görünümü ───\n");
        kurumsal.Display();

        // ✅ Aynı interface ile tek nesne
        Console.WriteLine("\n─── Tekil Ödeme (Leaf) Hesabı ───");
        IPaymentComponent tekil = temelPlan;
        Console.WriteLine($"   {tekil.Name}: {tekil.Calculate():C}");

        // ✅ Aynı interface ile paket hesabı — tip kontrolü yok
        Console.WriteLine("\n─── Paket Hesabı (Composite) ───");
        IPaymentComponent paket = kurumsal;
        Console.WriteLine($"   {paket.Name}: {paket.Calculate():C}");

        // ✅ Tüm ağaçta uniform işlem: kısmi iptal
        Console.WriteLine("\n─── Eklentiler Paketi İptal Ediliyor ───");
        eklentiler.Cancel();

        Console.WriteLine("\n─── İptal Sonrası Fatura ───\n");
        kurumsal.Display();

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ tekil.Calculate() ve paket.Calculate() aynı çağrı");
        Console.WriteLine("✓ İç içe paket derinliği sınırsız — kod değişmez");
        Console.WriteLine("✓ Cancel() tüm ağacı recursive dolaşıyor");
        Console.WriteLine("✓ Tip kontrolü (is SinglePayment?) hiç yok\n");
    }
}
