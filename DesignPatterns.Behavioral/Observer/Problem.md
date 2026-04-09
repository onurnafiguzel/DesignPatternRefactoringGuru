# Observer Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

Bir yatırım uygulaması geliştiriyorsunuz. **Hisse senedi fiyatı değiştiğinde**, birkaç bileşen haber alması ve harekete geçmesi gerekir:

1. **Portfolio Manager** — Portföyün güncel değerini hesapla
2. **Alert Service** — Eğer büyük fiyat hareketi varsa uyarı gönder
3. **Report Generator** — Fiyat değişmesini raporla

**Gereksinimler:**
- Stock sınıfı bu hizmetlerin detaylarını bilmemeli
- Hizmetler dinamik olarak subscribe/unsubscribe olabilmeli
- Hizmetler bağımsız çalışabilmeli (loose coupling)
- Yeni hizmet eklemek kolay olmalı

---

## ❌ PROBLEM: Pattern Olmadan

Bakın: [WrongApproach.cs](WrongApproach.cs)

### Sorun 1: Tight Coupling

```csharp
public class Stock
{
    // ❌ Stock direkt olarak hizmetleri bilmeli
    private Portfolio _portfolio;
    private AlertService _alertService;
    private ReportGenerator _reportGenerator;
    
    public void SetPrice(decimal newPrice)
    {
        _price = newPrice;
        
        // ❌ Stock'un tüm detaylarını bilmesi gerekir
        _portfolio.UpdateValue();
        _alertService.CheckPrice();
        _reportGenerator.LogChange();
    }
}
```

**Sorunlar:**
- Stock'u değiştirmek gerekirse, tüm bağlantıları kontrol etmeliyiz
- Stock, hizmetlerin constructor'ına ihtiyaç duyar
- Test yazmak zor (mock yapması gerekir)

### Sorun 2: Polling (Sürekli Kontrol)

```csharp
public class Portfolio
{
    public void Update()
    {
        while (true)
        {
            // ❌ Fiyat değiştiğini nasıl anlarız?
            // Constant polling = inefficient!
            CheckStockPrice();  // Şu anda değişti mi?
            Thread.Sleep(1000); // 1 saniyede bir kontrol et
        }
    }
}
```

**Sorunlar:**
- Gereksiz CPU tüketimi
- Delay'ler (1 saniye sonra fark edebilir)
- Scalable değil

### Sorun 3: Scalability (Yeni Hizmet Eklemek)

```csharp
public class Stock
{
    // ❌ Hizmetler sınırsız artarsa?
    private Portfolio _portfolio;
    private AlertService _alertService;
    private ReportGenerator _reportGenerator;
    private EmailNotifier _emailNotifier;           // Yeni
    private SlackNotifier _slackNotifier;           // Yeni
    private DatabaseLogger _databaseLogger;         // Yeni
    private AnalyticsEngine _analyticsEngine;       // Yeni
    // ... Ve daha fazlası
    
    public void SetPrice(decimal newPrice)
    {
        _price = newPrice;
        _portfolio.UpdateValue();
        _alertService.CheckPrice();
        _reportGenerator.LogChange();
        _emailNotifier.SendEmail();                // Yeni
        _slackNotifier.SendMessage();              // Yeni
        _databaseLogger.Log();                     // Yeni
        _analyticsEngine.Track();                  // Yeni
    }
}

// ❌ SetPrice metodu her hizmet ekleme ile büyüyor
// ❌ Open/Closed Principle ihlali
```

### Sorun 4: Responsibility (Sorumluluk Dağılımı)

```csharp
public class Stock
{
    public void SetPrice(decimal newPrice)
    {
        // ❌ Stock'un pek çok sorumluluğu var:
        // - Fiyat yönetimi
        // - Portfolio update
        // - Alert gönderme
        // - Rapor oluşturma
        // - Bildirim gönderme
        // - Analytics tracking
        // 
        // Bu çok fazla!
    }
}
```

**Sorun:**
- Single Responsibility Principle ihlali
- Teste zor
- Bakımı zor

### Sorun 5: Runtime Dinamikliği

```csharp
// ❌ Alert service'i geçici olarak kapatmak istiyoruz
// Ama nasıl? AlertService'i sildikten sonra yeniden derle?
_alertService = null;  // ❌ Çöp atıl, ama yeniden eklemek zor

// ❌ Sadece haftasonları rapor oluşturmak istiyoruz
// Ama SetPrice'a haftasonı kontrolü eklemeliyiz?
```

**Sorun:**
- Runtime'da subscribe/unsubscribe yok
- Koşullu çalışma logic karmaşık

---

## ✅ ÇÖZÜM: Observer Pattern

Bakın: [Pattern.cs](Pattern.cs)

### Felsefe: "Push-based, Loose Coupling"

```csharp
// ✅ Stock sadece fiyatını bilir, hizmetleri bilmez
public class Stock
{
    private List<IStockObserver> _observers = new();
    private decimal _price;
    
    public void Subscribe(IStockObserver observer)
    {
        _observers.Add(observer);
    }
    
    public void Unsubscribe(IStockObserver observer)
    {
        _observers.Remove(observer);
    }
    
    public decimal Price
    {
        set
        {
            if (_price != value)
            {
                _price = value;
                // ✅ Basit: Tüm observer'lara bildir
                NotifyObservers();
            }
        }
    }
    
    private void NotifyObservers()
    {
        // ✅ Hizmetlerin detaylarını bilmiyoruz
        // Sadece IStockObserver interface'ini biliyoruz
        foreach (var observer in _observers)
        {
            observer.OnPriceChanged(new StockPriceChangedEventArgs(...));
        }
    }
}

// ✅ Observer interface
public interface IStockObserver
{
    void OnPriceChanged(StockPriceChangedEventArgs args);
}

// ✅ Hizmetler bu interface'i implement ediyor
public class Portfolio : IStockObserver
{
    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        // Portfolio'yu update et
    }
}

public class AlertService : IStockObserver
{
    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        // Uyarı kontrol et ve gönder
    }
}

// ✅ Kullanım (Runtime'da subscribe/unsubscribe)
var stock = new Stock("APPLE", 150m);
var portfolio = new Portfolio();
var alertService = new AlertService();

stock.Subscribe(portfolio);
stock.Subscribe(alertService);

stock.Price = 165m;  // ✅ Otomatik olarak tüm observer'lar haberdar olur

stock.Unsubscribe(alertService);  // ✅ Runtime'da çıkar
stock.Price = 160m;  // ✅ Alert service bu mesajı görmez
```

### Avantajlar:

✅ **Loose Coupling** — Stock'u, hizmetleri tanımaz  
✅ **Push-based** — Polling yok, verimli  
✅ **Dynamic Subscription** — Runtime'da ekle/çıkar  
✅ **Scalability** — Yeni observer'lar eklemek kolay  
✅ **Open/Closed Principle** — Stock'u değiştirmeden yeni observer'lar ekle  
✅ **Separation of Concerns** — Her observer'ın tek sorumluluğu var  

---

## 📊 Karşılaştırma

### Pattern OLMADAN:

```
Stock                           (Çok Sorumluluğu Var)
├── Portfolio (direkt bağlı)
├── AlertService (direkt bağlı)
├── ReportGenerator (direkt bağlı)
├── EmailNotifier (direkt bağlı)
├── SlackNotifier (direkt bağlı)
└── ... (Hizmetler artarsa karmaşık)

SET PRICE → Update Portfolio → Send Alert → Generate Report → Send Email → ...

❌ Tight Coupling
❌ SetPrice uzun
❌ Yeni hizmet için değişim
❌ Test zor
```

### Pattern İLE:

```
Stock                           (Sadece Fiyat Yönetir)
└── _observers: IStockObserver[]
    ├── Portfolio (loose coupling)
    ├── AlertService (loose coupling)
    ├── ReportGenerator (loose coupling)
    ├── EmailNotifier (loose coupling)
    └── ... (Runtime'da ekle/çıkar)

SET PRICE → NotifyObservers() → Parallel (her observer kendi işini yap)

✅ Loose Coupling
✅ Clean
✅ Dynamic
✅ Test kolay
```

---

## 🔧 İmplementasyon Detayları

### Adım 1: Observer Interface

```csharp
public interface IStockObserver
{
    void OnPriceChanged(StockPriceChangedEventArgs args);
}
```

### Adım 2: Event Args

```csharp
public class StockPriceChangedEventArgs
{
    public string Symbol { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime ChangedAt { get; set; }
}
```

### Adım 3: Subject (Observer'ları Yönet)

```csharp
public class Stock
{
    private List<IStockObserver> _observers = new();
    
    public void Subscribe(IStockObserver observer)
    {
        _observers.Add(observer);
    }
    
    public void Unsubscribe(IStockObserver observer)
    {
        _observers.Remove(observer);
    }
    
    private void NotifyObservers(StockPriceChangedEventArgs args)
    {
        foreach (var observer in _observers)
        {
            observer.OnPriceChanged(args);
        }
    }
}
```

### Adım 4: Concrete Observer'lar

```csharp
public class Portfolio : IStockObserver
{
    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        // Portföy güncelle
    }
}

public class AlertService : IStockObserver
{
    public void OnPriceChanged(StockPriceChangedEventArgs args)
    {
        // Uyarı kontrol et
    }
}
```

---

## 💡 Ne Zaman Kullanılır?

- 📊 **Event-Driven Systems** — GUI frameworks (button click), MVC
- 💬 **Real-time Updates** — WebSocket, SignalR
- 📢 **Publish-Subscribe** — Message queues, Event bus
- 🔔 **Notifications** ← Örneğimiz
- 📈 **Real-time Analytics** — Data stream processing
- ⏲️ **Scheduler Events** — Timer callbacks

---

## ⚠️ Dikkat!

1. **Observer Sırası** — Observer'lar sırası garantili mi?
2. **Exception Handling** — Bir observer exception atarsa?
3. **Memory Leak** — Observer'ı unsubscribe etmezsen?
4. **Async Issues** — Multi-threaded environment'da?

Bakın: [Example.cs](Example.cs) — Tam çalışan örnek
