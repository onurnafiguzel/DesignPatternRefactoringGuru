# Builder Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/builder](https://refactoring.guru/design-patterns/builder) adresindeki senaryodan esinlenmiştir.

Bir inşaat uygulaması geliştiriyorsunuz. Sistem farklı tiplerde **ev** inşa edebiliyor: basit kulübe, normal ev, lüks villa. Her ev farklı bileşenlerden oluşuyor; kiminin garajı var, kiminin yüzme havuzu, kiminin bahçesi yok.

---

## ❌ PROBLEM: Pattern Olmadan

### Problem 1 — Dev Constructor

```csharp
public class House
{
    // ❌ Her kombinasyon için parametre
    public House(
        int floors,
        int rooms,
        bool hasGarage,
        bool hasSwimmingPool,
        bool hasGarden,
        bool hasSolarPanels,
        string roofType,
        string foundationType,
        int? parkingSpots = null,
        bool? hasBasement = null,
        string? facadeColor = null)
    { ... }
}

// ❌ Kullanım okunaksız — hangi true neyi temsil ediyor?
var house = new House(2, 5, true, false, true, false, "flat", "concrete", 1, null, "white");
```

### Problem 2 — Telescoping Constructor (Sıralı Kopyalar)

```csharp
public House(int floors, int rooms) { ... }
public House(int floors, int rooms, bool hasGarage) { ... }
public House(int floors, int rooms, bool hasGarage, bool hasPool) { ... }
// ❌ Kombinasyon sayısı patlıyor
```

### Problem 3 — Alt Sınıf Patlaması

```csharp
public class SimpleHouse : House { ... }
public class HouseWithGarage : House { ... }
public class HouseWithGarageAndPool : House { ... }
public class LuxuryHouseWithEverything : House { ... }
// ❌ Her kombinasyon için ayrı sınıf
```

### Sorunlar:

1. **Telescoping constructor** → İsteğe bağlı parametreler çoğaldıkça constructor'lar patlar
2. **Okunaksız nesne oluşturma** → `new House(2, 5, true, false, true, ...)` ne anlama gelir?
3. **Kısmi nesne durumu** → Setter'larla adım adım kurulursa nesne geçici tutarsız olabilir
4. **Tüm kombinasyonlar için alt sınıf** → Kod patlaması

---

## ✅ ÇÖZÜM: Builder Pattern

### Felsefe: "Karmaşık nesneyi adım adım inşa et, son adımda teslim al"

```
Director (HouseConstructor)
└── builder: IHouseBuilder
    ├── SimpleHouseBuilder  → basit kulübe
    ├── StandardHouseBuilder → normal ev
    └── LuxuryHouseBuilder  → lüks villa

Her builder:
  .BuildFoundation()
  .BuildWalls()
  .BuildRoof()
  .BuildGarage()      ← opsiyonel
  .BuildSwimmingPool() ← opsiyonel
  .GetResult() → House
```

### Kullanım:
```csharp
var director = new HouseDirector();

// Lüks villa inşa et
var builder = new LuxuryHouseBuilder();
director.Construct(builder);
House villa = builder.GetResult();

// Basit kulübe inşa et
var simpleBuilder = new SimpleHouseBuilder();
director.Construct(simpleBuilder);
House cabin = simpleBuilder.GetResult();

// Director olmadan, özel yapı
var customBuilder = new StandardHouseBuilder();
customBuilder.BuildFoundation().BuildWalls().BuildRoof().BuildGarage();
House custom = customBuilder.GetResult();
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | BUILDER |
|---------|---------|---------|
| **Okunabilirlik** | Düşük ❌ | Yüksek ✅ |
| **İsteğe bağlı adımlar** | Constructor şişer ❌ | Sadece gerekli adımlar ✅ |
| **Aynı süreç, farklı ürün** | Zor ❌ | Director ile ✅ |
| **Adım sırası kontrolü** | Yok ❌ | Director'da ✅ |
| **Nesne tutarlılığı** | Riskli ❌ | GetResult() garantiler ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 🏠 **Karmaşık nesne oluşturma** ← Örneğimiz
- 📄 **Query Builder** — SQL sorgusu adım adım oluşturma
- 📧 **Email oluşturma** — Alıcı, CC, ek, şablon, gövde
- 🍕 **Sipariş oluşturma** — Özelleştirilebilir ürünler
- 🖥️ **Test nesnesi oluşturma** — Object Mother / Test Data Builder
- 📊 **Rapor oluşturma** — Başlık, bölümler, grafik, özet

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
