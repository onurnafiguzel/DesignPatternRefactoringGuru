# Prototype Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/prototype](https://refactoring.guru/design-patterns/prototype) adresindeki senaryodan esinlenmiştir.

Bir grafik editörü geliştiriyorsunuz. Kullanıcılar `Circle`, `Rectangle`, `CompoundShape` gibi şekiller çiziyor. Seçilen şekli **Ctrl+D ile kopyalamak** istiyorlar — aynı renk, boyut ve pozisyona sahip yeni bir nesne.

---

## ❌ PROBLEM: Pattern Olmadan

### Problem 1 — Dışarıdan kopyalama imkansızdır

```csharp
public class Circle
{
    public double Radius { get; set; }
    public string Color { get; set; }
    private double _cachedArea;   // ❌ private alan, dışarıdan okunamaz

    public Circle(double radius, string color)
    {
        Radius = radius;
        Color = color;
        _cachedArea = Math.PI * radius * radius;  // Hesaplama var
    }
}

// ❌ Kopya almak istiyoruz ama nasıl?
Shape selected = GetSelectedShape();  // Runtime'da Circle mı? Rectangle mı?

// ❌ Somut tipi bilmek zorundayız
if (selected is Circle c)
    new Circle(c.Radius, c.Color);          // _cachedArea kopyalanamadı
else if (selected is Rectangle r)
    new Rectangle(r.Width, r.Height, r.Color);
// ❌ Her yeni şekil tipi için buraya if/else eklenir
```

### Problem 2 — Interface üzerinden kopyalama yapılamaz

```csharp
public interface IShape { void Draw(); }

public void DuplicateShape(IShape shape)
{
    // ❌ IShape interface'i üzerinden new çağıramayız
    // ❌ Somut tipi bilmeden kopya alamayız
    // ❌ Reflection ile yapmak: kırılgan ve yavaş
}
```

### Sorunlar:

1. **Private alanlar kopyalanamaz** → Nesnenin tüm state'ine erişilemez
2. **Somut tip bağımlılığı** → Kopyalayan kod tüm alt tipleri bilmek zorunda
3. **Yeni tip eklemek** → Kopyalama kodundaki if/else'i güncelle
4. **Pahalı initialization tekrar** → DB'den yüklenmiş, hesaplanmış veriler yeniden işlenir
5. **Derin kopya (deep copy) karmaşıklığı** → İç içe nesneleri elle klonlamak hatalara açık

---

## ✅ ÇÖZÜM: Prototype Pattern

### Felsefe: "Kopyalanma sorumluluğunu nesnenin kendisine ver"

```csharp
public interface IShape
{
    IShape Clone();   // ✅ Her nesne kendini kopyalar
    void Draw();
}

public class Circle : IShape
{
    public IShape Clone() => new Circle(this);  // ✅ Copy constructor
}

// Artık somut tipi bilmeden kopyalayabiliriz:
IShape copy = selectedShape.Clone();  // Circle mı Rectangle mı? Fark etmez!
```

### Kullanım:
```csharp
// Orijinal şekiller
var circle = new Circle(10, "Kırmızı", x: 50, y: 100);
var rect   = new Rectangle(200, 100, "Mavi", x: 300, y: 150);

// ✅ Klonla — somut tipi bilmeden
IShape circleCopy = circle.Clone();
IShape rectCopy   = rect.Clone();

// ✅ Şekil kaydı (Prototype Registry)
var registry = new ShapeRegistry();
registry.Register("kırmızı-daire", circle);

IShape preset = registry.Clone("kırmızı-daire");  // Hazır prototype'tan üret
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | PROTOTYPE |
|---------|---------|-----------|
| **Private alan kopyalama** | İmkansız ❌ | Sınıf kendi içinde ✅ |
| **Somut tip bağımlılığı** | Zorunlu ❌ | Yok ✅ |
| **Yeni tip eklemek** | if/else güncelle ❌ | Sadece Clone() ekle ✅ |
| **Pahalı init tekrarı** | Her seferinde ❌ | Klonda atlanır ✅ |
| **Derin kopya** | Elle, hatalara açık ❌ | Kapsüllenmiş ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 🎨 **Grafik editörler** ← Örneğimiz (Ctrl+D ile şekil kopyalama)
- 📄 **Belge şablonları** — Hazır template'den yeni belge oluşturma
- 🎮 **Oyun nesneleri** — Düşman spawn, mermi kopyalama
- 🗄️ **Pahalı veritabanı nesneleri** — Yeniden yüklemek yerine klonla
- 🧪 **Test fixture'ları** — Test verisi hazır nesneden türetme
- ⚙️ **Konfigürasyon nesneleri** — Default config'den özelleştirilmiş kopya

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
