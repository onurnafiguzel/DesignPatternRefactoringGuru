# C# Tasarım Kalıpları (Design Patterns) — Öğrenme Projesi

Hoşgeldiniz! Bu proje, Gang of Four (GoF) tasarım kalıplarını **problem-çözüm yaklaşımıyla** öğretmek amacıyla tasarlanmıştır.

## 📚 Yapı

```
DesignPatterns/
├── DesignPatterns.Creational/          # Yaratımsal Kalıplar
│   ├── Singleton/
│   │   ├── Problem.md                  # Sorun analizi
│   │   ├── WrongApproach.cs            # Pattern olmadan ❌
│   │   ├── Pattern.cs                  # Pattern ile ✅
│   │   └── Example.cs                  # Pratik örnek + testler
│   └── README.md
│
├── DesignPatterns.Structural/          # Yapısal Kalıplar
│   ├── Adapter/
│   │   ├── Problem.md
│   │   ├── WrongApproach.cs
│   │   ├── Pattern.cs
│   │   └── Example.cs
│   └── README.md
│
└── DesignPatterns.Behavioral/          # Davranışsal Kalıplar
    ├── Observer/
    │   ├── Problem.md
    │   ├── WrongApproach.cs
    │   ├── Pattern.cs
    │   └── Example.cs
    └── README.md
```

## 🎓 Her Kalıp İçin Dosya Yapısı

### 1. **Problem.md** — Sorun Analizi
- Senaryo ve gereksinimler
- Sorunlar (pattern olmadan)
- Çözüm (pattern ile)
- Avantajlar vs. Dezavantajlar
- Ne zaman kullanılacağı

### 2. **WrongApproach.cs** — Kötü Yaklaşım
```csharp
// ❌ Pattern olmadan nasıl sorun oluştuğunu gösterir
// - Tight coupling
// - Kaynakların israfi
// - Testleme zorlukları
// - Bakım zorlukları
```

### 3. **Pattern.cs** — Doğru Yaklaşım
```csharp
// ✅ Pattern ile nasıl çözeceğini gösterir
// - Loose coupling
// - Clean code
// - Testlenebilirlik
// - Bakım kolaylığı
```

### 4. **Example.cs** — Pratik Örnekler
```csharp
// Karşılaştırmalı çalışan örnek
// - Pattern olmadan vs. Pattern ile
// - Real-world senaryo
// - Unit test örnekleri
// - Advanced kullanımlar
```

---

## 🚀 Hızlı Başlangıç

### Solution'u Açma

```bash
# Terminal'de
cd c:\Users\USER\source\repos\DesignPatterns

# Visual Studio
start DesignPatterns.sln

# veya CLI ile
dotnet sln DesignPatterns.sln list
```

### Projeleri Derleme

```bash
dotnet build
```

### Örnek Çalıştırma (Henüz konsol app yok, class library'dir)

Eğer console app istiyorsanız, şu adımları takip edin:

1. Console App Projesi Oluştur:
```bash
dotnet new console -n DesignPatterns.ConsoleApp
```

2. Yeni Program.cs:
```csharp
using DesignPatterns.Creational.Singleton.Examples;
using DesignPatterns.Structural.Adapter.Examples;
using DesignPatterns.Behavioral.Observer.Examples;

Console.WriteLine("╔════════════════════════════════════════════════════════╗");
Console.WriteLine("║        C# Tasarım Kalıpları - Öğrenme Projesi         ║");
Console.WriteLine("╚════════════════════════════════════════════════════════╝");

Console.WriteLine("\nHangi kalıbı incelemek istiyorsunuz?\n");
Console.WriteLine("1. Singleton (Creational)");
Console.WriteLine("2. Adapter (Structural)");
Console.WriteLine("3. Observer (Behavioral)");
Console.Write("\nSeçim (1-3): ");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        SingletonExample.RunAll();
        break;
    case "2":
        AdapterExample.RunAll();
        break;
    case "3":
        ObserverExample.RunAll();
        break;
    default:
        Console.WriteLine("Geçersiz seçim!");
        break;
}
```

3. Çalıştır:
```bash
cd DesignPatterns.ConsoleApp
dotnet run
```

---

## 📖 Öğrenme Yolu

### Başlangıç Seviyesi
1. **Singleton** ← Başlayın, en basit pattern
   - Dosya: [Creational/Singleton/Problem.md](DesignPatterns.Creational/Singleton/Problem.md)
   - Problem anlaşılır, çözüm açık

### Orta Seviye
2. **Adapter** ← Structural pattern
   - Dosya: [Structural/Adapter/Problem.md](DesignPatterns.Structural/Adapter/Problem.md)
   - Interface uyumsuzluğu ve çevirme konsepti

3. **Observer** ← Behavioral pattern
   - Dosya: [Behavioral/Observer/Problem.md](DesignPatterns.Behavioral/Observer/Problem.md)
   - Event-driven ve loose coupling

### İleri Seviye
Ilerideki katkılar ile:
- Factory Method / Abstract Factory (Creational)
- Bridge, Composite, Decorator (Structural)
- Strategy, State, Command (Behavioral)

---

## 💡 Her Kalıp İçin Temel Bilgiler

### Singleton (Creational)
**Senaryo:** Bir veritabanı bağlantı pool'unun sadece bir örneğinin olması

**Pattern Olmadan Sorunlar:**
- Kontrol dışı örnek oluşturma
- Kaynak israfı
- Thread-safety sorunları

**Pattern İle Çözüm:**
- Private constructor
- Static instance (Lazy<T>)
- Thread-safe erişim

📍 Dosya: [DesignPatterns.Creational/Singleton/](DesignPatterns.Creational/Singleton/)

---

### Adapter (Structural)
**Senaryo:** Email ve SMS provider'ları aynı interface ile kullanma

**Pattern Olmadan Sorunlar:**
- Interface uyumsuzluğu
- Type checking ve if/else zinciri
- Tight coupling

**Pattern İle Çözüm:**
- Adapter sınıfları ile çeviri
- Tek interface ile çoklu kanal
- Loose coupling

📍 Dosya: [DesignPatterns.Structural/Adapter/](DesignPatterns.Structural/Adapter/)

---

### Observer (Behavioral)
**Senaryo:** Hisse senedi fiyatı değiştiğinde birden fazla bileşeni haberdar etme

**Pattern Olmadan Sorunlar:**
- Tight coupling
- Polling (verimsiz)
- Yeni bileşen eklemek zor

**Pattern İle Çözüm:**
- Push-based notification
- Loose coupling
- Dynamic subscribe/unsubscribe

📍 Dosya: [DesignPatterns.Behavioral/Observer/](DesignPatterns.Behavioral/Observer/)

---

## 🎯 Öğrenme İpuçları

1. **Problem.md'yi İlk Okuyin**
   - Senaryoyu anlayın
   - Sorunları belirleyin
   - Neden bu pattern gerekli?

2. **WrongApproach.cs'i İncelypin**
   - Sorunları canlı kodda görelim
   - Çalışır ama problemli
   - Dikkat: Nasıl kodu karmaşıklaştırır?

3. **Pattern.cs'i Okuyun**
   - Çözüm nasıl uygulanır?
   - Avantajlar nelerdir?
   - Clean code yazım şekli

4. **Example.cs'i Çalıştırın**
   - Karşılaştırmalı görelim
   - Real-world senaryolar
   - Pratik kullanım

---

## 🔧 Teknik Yapı

**Target Framework:** .NET 8.0
**Language Features:** 
- `#nullable enable`
- `#nullable enable implicit usings`
- Latest C# features

**Namespace Kuralları:**
```csharp
namespace DesignPatterns.{Category}.{PatternName}
{
    // Category: Creational, Structural, Behavioral
    // PatternName: Singleton, Adapter, Observer, ...
}
```

---

## 📝 Senaryo Tasarım Felsefesi

Her pattern senaryo, **gerçek-hayat uygulamalarından esinlendi**:

- **Singleton** → Veritabanı bağlantı havuzu (gerçek)
- **Adapter** → Multi-channel notification sistemi (gerçek)
- **Observer** → Real-time stock monitoring (gerçek)

Oyuncak örnekler değil, production'da kullanılacak tasarımlar!

---

## 🚀 İlerideki Planlar

Şu kalıplar eklenecek:

### Creational
- [ ] Factory Method
- [ ] Abstract Factory
- [ ] Builder
- [ ] Prototype

### Structural
- [ ] Bridge
- [ ] Composite
- [ ] Decorator
- [ ] Facade
- [ ] Proxy
- [ ] Flyweight

### Behavioral
- [ ] Strategy
- [ ] State
- [ ] Command
- [ ] Iterator
- [ ] Mediator
- [ ] Memento
- [ ] Template Method
- [ ] Visitor
- [ ] Chain of Responsibility

---

## ❓ Sık Sorulan Sorular

### S: "Pattern olmadan niye gerçek kod kullanılıyor?"
A: Çünkü sorunları anlamak için. Oyuncak kod değil, production'da karşılaşacağız.

### S: "İlk olarak hangisini öğrenmeli?"
A: Singleton → Adapter → Observer sırasını takip edin.

### S: "Hangi pattern en önemli?"
A: Tümü önemli, ama Observer ve Strategy en sık kullanılanlar.

### S: "Console app neden yok?"
A: Modular yapı için class library. İsteyen kendi console app'i oluşturabilir.

### S: "Bu pattern'leri nerede kullanırım?"
A: Açıklamalar içinde "Ne zaman kullanılır?" bölümü var.

---

## 📚 Kaynaklar

- Gang of Four: Design Patterns (Temel kaynak)
- Microsoft Docs: Design Patterns
- Refactoring Guru: Design Patterns

---

## 💬 Katkı

Yeni pattern önerileriniz için feedback bekliyorum!

Başarılar! 🚀

---

**Oluşturma Tarihi:** Nisan 2025
**Version:** 1.0 (3 Pattern ile başladık)
**Status:** Active Development
