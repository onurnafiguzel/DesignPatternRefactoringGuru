# Abstract Factory Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/abstract-factory](https://refactoring.guru/design-patterns/abstract-factory) adresindeki senaryodan esinlenmiştir.

Bir cross-platform UI framework geliştiriyorsunuz. Uygulama **Windows**, **macOS** ve **Linux**'ta çalışıyor. Her platform için görsel olarak tutarlı bileşenler gerekiyor:

| Bileşen | Windows | macOS | Linux |
|---------|---------|-------|-------|
| `Button` | WinButton | MacButton | LinuxButton |
| `Checkbox` | WinCheckbox | MacCheckbox | LinuxCheckbox |
| `TextInput` | WinTextInput | MacTextInput | LinuxTextInput |

**Kural:** Aynı uygulama içinde Windows Button ile Mac Checkbox bir arada **olmamalı** — aile tutarlılığı zorunlu.

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class Application
{
    private string _os;

    public void RenderUI()
    {
        // ❌ Bileşenler if/else ile seçiliyor
        if (_os == "Windows")
        {
            var btn      = new WinButton();
            var checkbox = new WinCheckbox();
            var input    = new WinTextInput();
            btn.Render(); checkbox.Render(); input.Render();
        }
        else if (_os == "macOS")
        {
            var btn      = new MacButton();
            var checkbox = new MacCheckbox();
            var input    = new MacTextInput();
            btn.Render(); checkbox.Render(); input.Render();
        }
        else if (_os == "Linux")
        {
            // ❌ Yeni platform = tüm bu bloğu yaz
        }
    }
}
```

### Sorunlar:

1. **Aile tutarlılığı garanti değil** → Hata ile `WinButton` + `MacCheckbox` bir arada olabilir
2. **Yeni platform eklemek** → `RenderUI`'deki her if/else bloğunu güncelle
3. **Yeni bileşen tipi eklemek** → Her platform bloğuna yeni satır ekle
4. **Kod tekrarı** → Her platform bloğu aynı yapıyı tekrarlıyor
5. **Test zorluğu** → Platform kontrolü iş mantığına karışmış

---

## ✅ ÇÖZÜM: Abstract Factory

### Felsefe: "Ailenin tüm üyelerini üreten factory'yi soyutla"

```
IUIFactory (abstract factory)
├── CreateButton()
├── CreateCheckbox()
└── CreateTextInput()

    ├── WindowsUIFactory  → Win* ailesini üretir
    ├── MacUIFactory      → Mac* ailesini üretir
    └── LinuxUIFactory    → Linux* ailesini üretir
```

### Kullanım:
```csharp
// Factory seçimi tek noktada, bir kez
IUIFactory factory = Environment.OSVersion.Platform switch
{
    PlatformID.Win32NT => new WindowsUIFactory(),
    PlatformID.MacOSX  => new MacUIFactory(),
    _                  => new LinuxUIFactory()
};

// Application factory'yi alır, somut tipleri bilmez
var app = new Application(factory);
app.RenderUI();
// ✅ Aile tutarlılığı garantili — Mac factory her zaman Mac bileşeni üretir
```

---

## Factory Method vs Abstract Factory

| | Factory Method | Abstract Factory |
|---|---|---|
| **Üretilen** | Tek ürün | Birbiriyle uyumlu ürün ailesi |
| **Soyutlama** | Tek metod | Birden fazla metod |
| **Soru** | *Hangi nesneyi üretelim?* | *Hangi aileyi kullanalım?* |
| **Örnek** | `CreateTransport()` | `CreateButton() + CreateCheckbox()` |

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | ABSTRACT FACTORY |
|---------|---------|-----------------|
| **Aile tutarlılığı** | Garanti yok ❌ | Garantili ✅ |
| **Yeni platform** | Her bloğu güncelle ❌ | Yeni factory sınıfı ✅ |
| **Yeni bileşen** | Her platforma ekle ❌ | Interface + her factory ✅ |
| **Test / Mock** | Zor ❌ | Mock factory ✅ |
| **Open/Closed** | İhlal ❌ | Sağlı ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 🖥️ **Cross-platform UI** ← Örneğimiz
- 🎨 **Tema sistemleri** — Dark/Light/High-contrast bileşen aileleri
- 🗄️ **Database abstraction** — MySQL/PostgreSQL/SQLite driver ailesi
- 🧪 **Test doubles** — Gerçek vs Mock servis ailesi
- 🌍 **Lokalizasyon** — Bölgeye göre format, para birimi, takvim ailesi
- 🎮 **Oyun bölümleri** — Düşman/zemin/arka plan ailesi (dünya teması)

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
