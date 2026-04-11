namespace DesignPatterns.Creational.Prototype.Correct;

/// <summary>
/// ✅ PROTOTYPE PATTERN: Kopyalanma sorumluluğunu nesneye ver
///
/// Amaç:
/// - Mevcut nesneleri kopyalayarak yeni nesneler oluştur
/// - Kopyalayan kod somut tipi bilmek zorunda kalmasın
/// - Private alanlar dahil tüm state kopyalansın
/// - Pahalı initialization'ı tekrarlama
///
/// Kaynak: https://refactoring.guru/design-patterns/prototype
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Prototype Interface
// ═══════════════════════════════════════════════════════════

public abstract class Shape
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Color { get; set; }

    // Somut sınıflar için base constructor
    protected Shape(int x, int y, string color)
    {
        X = x; Y = y; Color = color;
    }

    // ✅ Copy constructor — alt sınıflar base(other) ile çağırır
    protected Shape(Shape other)
    {
        X = other.X;
        Y = other.Y;
        Color = other.Color;
    }

    // ✅ Her alt sınıf kendini kopyalar — override zorunda
    public abstract Shape Clone();

    public abstract void Draw();

    protected string Position => $"({X},{Y})";
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Concrete Prototypes
// ═══════════════════════════════════════════════════════════

public class Circle : Shape
{
    public double Radius { get; set; }

    // private — dışarıdan erişilemeyen hesaplanmış alan
    private double _cachedArea;

    public Circle(double radius, string color, int x = 0, int y = 0)
        : base(x, y, color)
    {
        Radius = radius;
        _cachedArea = Math.PI * radius * radius;  // "Pahalı" hesaplama
        Console.WriteLine($"   [Circle oluşturuldu — alan hesaplandı: {_cachedArea:F2}]");
    }

    // ✅ Copy constructor: private _cachedArea da kopyalanıyor
    private Circle(Circle other) : base(other)
    {
        Radius = other.Radius;
        _cachedArea = other._cachedArea;   // ✅ Hesaplamayı tekrarlamadık
    }

    public override Shape Clone() => new Circle(this);

    public override void Draw() =>
        Console.WriteLine($"   ⭕ Circle  r={Radius}  renk={Color}  konum={Position}  alan={_cachedArea:F2}");
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height, string color, int x = 0, int y = 0)
        : base(x, y, color)
    {
        Width = width; Height = height;
        Console.WriteLine($"   [Rectangle oluşturuldu]");
    }

    private Rectangle(Rectangle other) : base(other)
    {
        Width = other.Width;
        Height = other.Height;
    }

    public override Shape Clone() => new Rectangle(this);

    public override void Draw() =>
        Console.WriteLine($"   ▭  Rect    {Width}×{Height}  renk={Color}  konum={Position}");
}

// Bileşik şekil: deep copy örneği için
public class CompoundShape : Shape
{
    // ✅ Deep copy gereken iç koleksiyon
    private List<Shape> _children;

    public CompoundShape(string color, int x = 0, int y = 0)
        : base(x, y, color)
    {
        _children = new List<Shape>();
    }

    private CompoundShape(CompoundShape other) : base(other)
    {
        // ✅ Deep copy: her child da Clone() ile kopyalanıyor
        _children = other._children
            .Select(child => child.Clone())
            .ToList();
    }

    public void Add(Shape shape) => _children.Add(shape);

    public override Shape Clone() => new CompoundShape(this);

    public override void Draw()
    {
        Console.WriteLine($"   📦 Compound  renk={Color}  konum={Position}  ({_children.Count} şekil)");
        foreach (var child in _children)
        {
            Console.Write("      └─ ");
            child.Draw();
        }
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Prototype Registry (Hazır şablonları sakla)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Sık kullanılan prototype'ları isme göre saklayan kayıt.
/// "Pahalı" nesneleri bir kez oluştur, isteyene klonla ver.
/// </summary>
public class ShapeRegistry
{
    private Dictionary<string, Shape> _prototypes = new();

    public void Register(string key, Shape prototype)
    {
        _prototypes[key] = prototype;
        Console.WriteLine($"   [Registry] '{key}' kaydedildi");
    }

    // ✅ Her çağrıda yeni oluşturmak yerine klon döner
    public Shape Clone(string key)
    {
        if (!_prototypes.TryGetValue(key, out var prototype))
            throw new KeyNotFoundException($"'{key}' kayıtlı değil");

        Console.WriteLine($"   [Registry] '{key}' klonlandı");
        return prototype.Clone();
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class PrototypePatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║      ✅ PROTOTYPE PATTERN — Grafik Editörü              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        // Orijinal şekiller (initialization maliyeti burada)
        Console.WriteLine("\n─── Orijinal Şekiller Oluşturuluyor ───\n");
        var circle = new Circle(radius: 30, color: "Kırmızı", x: 100, y: 200);
        var rect   = new Rectangle(width: 150, height: 80, color: "Mavi", x: 300, y: 150);

        // Bileşik şekil
        var compound = new CompoundShape("Yeşil", x: 50, y: 50);
        compound.Add(new Circle(10, "Sarı", 10, 10));
        compound.Add(new Rectangle(40, 20, "Mor", 30, 30));

        Console.WriteLine("\n─── Orijinaller ───");
        circle.Draw();
        rect.Draw();
        compound.Draw();

        // ✅ Ctrl+D: Kopyala — somut tipi bilmeden
        Console.WriteLine("\n─── Klonlar (Ctrl+D) — initialization maliyeti yok ───\n");
        Shape circleCopy   = circle.Clone();
        Shape rectCopy     = rect.Clone();
        Shape compoundCopy = compound.Clone();

        // Klonları kaydır
        circleCopy.X += 20; circleCopy.Y += 20;
        rectCopy.X   += 20; rectCopy.Color = "Turuncu";   // Rengi değiştir

        Console.WriteLine("Klonlar (orijinalden bağımsız):");
        circleCopy.Draw();
        rectCopy.Draw();
        compoundCopy.Draw();

        // ✅ Orijinal bozulmadı
        Console.WriteLine("\nOrijinal hâlâ sağlam:");
        circle.Draw();
        rect.Draw();

        // ✅ Prototype Registry
        Console.WriteLine("\n─── Prototype Registry ───\n");
        var registry = new ShapeRegistry();
        registry.Register("standart-daire",  new Circle(20, "Gri"));
        registry.Register("standart-kutu",   new Rectangle(100, 60, "Siyah"));

        Console.WriteLine();
        var preset1 = registry.Clone("standart-daire");
        var preset2 = registry.Clone("standart-daire");   // Aynı prototype, farklı klon
        preset1.Color = "Pembe";

        preset1.Draw();
        preset2.Draw();   // preset2 hâlâ Gri — bağımsız kopya

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ Clone() çağrısı somut tipi bilmiyor");
        Console.WriteLine("✓ Private _cachedArea yeniden hesaplanmadı");
        Console.WriteLine("✓ CompoundShape deep copy otomatik (recursive Clone)");
        Console.WriteLine("✓ Registry: pahalı nesneyi bir kez oluştur, çok kullan\n");
    }
}
