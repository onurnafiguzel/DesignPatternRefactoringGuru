# State Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

Bir e-ticaret uygulamasında sipariş yönetimi geliştiriyorsunuz. Siparişin birden fazla durumu var ve her durumda farklı işlemler geçerli:

| Durum             | İptal | Kargo | Teslim | İade |
|------------------ |-------|-------|--------|------|
| **Beklemede**     | ✅   | ✅    | ❌ | ❌ |
| **Kargoda**       | ❌   | ❌     | ✅ | ❌ |
| **Teslim Edildi** | ❌   | ❌     | ❌ | ✅ |
| **İptal Edildi**  | ❌   | ❌     | ❌ | ❌ |

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class Order
{
    public string Status { get; private set; } = "Pending";

    public void Cancel()
    {
        if (Status == "Pending")
            Status = "Cancelled";
        else if (Status == "Shipped")
            throw new InvalidOperationException("Kargodaki sipariş iptal edilemez!");
        else if (Status == "Delivered")
            throw new InvalidOperationException("Teslim edilmiş sipariş iptal edilemez!");
        // ❌ Yeni durum eklenirse buraya if/else eklenir
    }

    public void Ship()
    {
        if (Status == "Pending")
            Status = "Shipped";
        else if (Status == "Shipped")
            throw new InvalidOperationException("Zaten kargoda!");
        else if (Status == "Delivered")
            throw new InvalidOperationException("Zaten teslim edildi!");
        // ❌ Her metotta aynı durum kontrolleri tekrar ediyor
    }

    public void Deliver()
    {
        if (Status == "Shipped")
            Status = "Delivered";
        else
            throw new InvalidOperationException("Önce kargoya verilmeli!");
    }
}
```

### Sorunlar:

1. **Durum kontrolleri her metotta tekrar ediyor** → DRY ihlali
2. **Yeni durum eklemek** → Her metoddaki if/else bloklarını güncelle
3. **Geçerli geçişleri takip etmek zor** → Hangi durumda ne yapılabilir?
4. **Single Responsibility ihlali** → Order sınıfı hem iş mantığı hem durum geçişini yönetiyor
5. **Karmaşık büyüme** → 10 durum × 10 işlem = 100 if/else bloğu

---

## ✅ ÇÖZÜM: State Pattern

### Felsefe: "Her durumu ayrı bir sınıfa kapsülle"

```
Order (Context)
└── _state: IOrderState
    ├── PendingState    → cancel ✅, ship ✅, deliver ❌, refund ❌
    ├── ShippedState    → cancel ❌, ship ❌, deliver ✅, refund ❌
    ├── DeliveredState  → cancel ❌, ship ❌, deliver ❌, refund ✅
    └── CancelledState  → tüm işlemler ❌
```

### Kullanım:
```csharp
var order = new Order("ORD-001");

order.Ship();     // ✅ Pending → Shipped
order.Deliver();  // ✅ Shipped → Delivered
order.Cancel();   // ❌ "Teslim edilmiş sipariş iptal edilemez"
order.Refund();   // ✅ Delivered → Refunded
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | STATE |
|---------|---------|-------|
| **Yeni durum ekleme** | Her metodu güncelle ❌ | Yeni sınıf ekle ✅ |
| **Durum kuralları** | Dağınık if/else ❌ | Tek sınıfta ✅ |
| **Geçerli geçişler** | Takip zor ❌ | Net ve açık ✅ |
| **Kod tekrarı** | Çok ❌ | Yok ✅ |
| **Test** | Zor ❌ | Her durum izole ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 📦 **Sipariş yönetimi** ← Örneğimiz
- 🚦 **İş akışı / onay süreçleri** — Taslak → İnceleme → Yayın
- 🎮 **Oyun karakteri durumları** — Sağlıklı, Yaralı, Ölü
- 🔌 **Bağlantı yönetimi** — Bağlanıyor, Bağlı, Bağlantı Kesildi
- 🏧 **ATM makinesi** — Bekleme, Kart Takılı, PIN Girişi, İşlem

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
