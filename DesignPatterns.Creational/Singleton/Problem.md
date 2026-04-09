# Singleton Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

Bir finansal uygulama geliştiriyorsunuz. Veritabanına bağlanmak için bir **bağlantı pool'u** kullanılıyor. 

**Gereksinimler:**
- Bağlantı pool'u uygulama başında bir kez oluşturulmalı
- Tüm yerden aynı pool örneğine erişilmeli
- Birden fazla bağlantı açılabilir, ama pool tek olmalı
- Thread-safe olmalı

---

## ❌ PROBLEM: Pattern Olmadan

Bakın: [WrongApproach.cs](WrongApproach.cs)

```csharp
var pool1 = new DatabaseConnectionPool();  // ❌ Pool oluştur
var pool2 = new DatabaseConnectionPool();  // ❌ Başka pool?
var pool3 = new DatabaseConnectionPool();  // ❌ Daha başka?

// Sonuç: 3 farklı pool, kontrol edilmiyor!
```

### Sorunlar:

1. **Kontrol eksikliği** → Herhangi yerde `new` yazılırsa yeni örnek oluşur
2. **Kaynak israfı** → Birden fazla pool açılır, bağlantılar karmaşıklaşır
3. **Veri tutarsızlığı** → Her pool kendi state'ini tutuyor
4. **Yönetim zorluğu** → Hangi pool'dan bağlantı açıyorsunuz?
5. **Thread-safety** → Race conditions oluşabilir

### Gerçek Hayat Senaryosu:

```csharp
public class UserRepository
{
    public void GetUser(int id)
    {
        var pool = new DatabaseConnectionPool();
        pool.OpenConnection("user-db");
        // ...
    }
}

public class OrderRepository
{
    public void GetOrder(int id)
    {
        var pool = new DatabaseConnectionPool();  // ❌ Başka pool!
        pool.OpenConnection("order-db");
        // ...
    }
}

// Sonuç: İki farklı pool, yönetim karmaşık, bağlantılar kontrol dışı
```

---

## ✅ ÇÖZÜM: Singleton Pattern

Bakın: [Pattern.cs](Pattern.cs)

### Singleton Felsefesi:

1. **Private Constructor** → Dışarıdan `new` ile oluşturulamasın
2. **Static Singleton** → Sadece bir örnek olsun
3. **Thread-Safe** → Concurrent access'te sorun olmasın
4. **Lazy Initialization** → İhtiyaç olduğunda oluşsun

```csharp
public sealed class DatabaseConnectionPool
{
    // Thread-safe, lazy initialization
    private static readonly Lazy<DatabaseConnectionPool> _instance = 
        new(() => new DatabaseConnectionPool());
    
    // Private constructor
    private DatabaseConnectionPool() { }
    
    // Global erişim noktası
    public static DatabaseConnectionPool Instance => _instance.Value;
}

// Kullanım:
var pool1 = DatabaseConnectionPool.Instance;
var pool2 = DatabaseConnectionPool.Instance;
// pool1 ve pool2 tamamen aynı! (ReferenceEquals = true)
```

### Avantajlar:

✅ **Tek örnek garantisi** → Kontrol içinde  
✅ **Global erişim** → `Instance` property ile  
✅ **Lazy initialization** → İlk kullanımda oluşturulur  
✅ **Thread-safe** → Concurrent ortamlarda güvenli  
✅ **sealed sınıf** → Inheritance'tan korunma  

---

## 📊 Karşılaştırma

| Özellik | Olmadan | Singleton |
|---------|---------|-----------|
| **Örnek Sayısı** | Sınırsız ❌ | 1 ✅ |
| **Kontrol** | Yok | Tam |
| **Thread-Safe** | Değil | Evet |
| **Resource Management** | Zor | Kolay |
| **Testlenebilirlik** | Zor | Orta* |

*Not: Interface ile Singleton'ı sarmalarsanız testlenebilirlik artar.

---

## 🔧 İmplementasyon Detayları

### Neden `Lazy<T>`?

```csharp
// Thread-safe initialization garantiler
private static readonly Lazy<DatabaseConnectionPool> _instance = 
    new(() => new DatabaseConnectionPool());

// İlk erişimde oluşturulur, sonrasında cache'lenir
// Double-checked locking otomatik
public static DatabaseConnectionPool Instance => _instance.Value;
```

### Neden `sealed`?

```csharp
public sealed class DatabaseConnectionPool  // sealed = kalıtılanamaz
{
    // Eğer sealed olmasaydı:
    // public class FakeDatabaseConnectionPool : DatabaseConnectionPool { }
    // Bu Singleton'ı kırabilir!
}
```

---

## 💡 Ne Zaman Kullanılır?

- 📊 **Database Connection Pool** ← Örneğimiz
- 📝 **Logger** — Uygulamada tek log'a yazmalısınız
- ⚙️ **Configuration Manager** — Ayarlar bir kez yüklenir
- 🧵 **Thread Pool** — OS thread pool'u tek olmalı
- 💾 **Cache Manager** — Shared cache single instance

---

## ⚠️ Dikkat!

1. **Over-use** → Her şey Singleton olmasın! (Global state'e çevrilir)
2. **Testing** → Testlerde zor. Interface ile sarıp, mock'layın
3. **Dependency Injection** → Modern C#'da DI container'ı tercih edin

Bakın: [Example.cs](Example.cs) — Tam çalışan örnek
