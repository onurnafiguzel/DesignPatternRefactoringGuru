# Strategy Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

Bir e-ticaret uygulamasında ödeme sistemi geliştiriyorsunuz. Müşteriler farklı ödeme yöntemleri kullanabiliyor:

- **Kredi Kartı** — Kart numarası, CVV, son kullanma tarihi doğrulaması
- **PayPal** — E-posta ve şifre ile yönlendirme
- **Kripto Para** — Cüzdan adresi ve blockchain onayı

Her ödeme yönteminin farklı bir algoritması var, ama hepsi aynı amacı gerçekleştiriyor: **ödemeyi tamamlamak**.

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class PaymentService
{
    public void ProcessPayment(decimal amount, string method)
    {
        if (method == "creditcard")
        {
            Console.WriteLine("Kart numarası doğrulanıyor...");
            Console.WriteLine("CVV kontrol ediliyor...");
            Console.WriteLine($"{amount:C} kredi kartından çekildi.");
        }
        else if (method == "paypal")
        {
            Console.WriteLine("PayPal'a yönlendiriliyor...");
            Console.WriteLine("E-posta / şifre doğrulanıyor...");
            Console.WriteLine($"{amount:C} PayPal'dan çekildi.");
        }
        else if (method == "crypto")
        {
            Console.WriteLine("Cüzdan adresi kontrol ediliyor...");
            Console.WriteLine("Blockchain onayı bekleniyor...");
            Console.WriteLine($"{amount:C} kripto cüzdanından çekildi.");
        }
        // ❌ Yeni ödeme yöntemi eklemek = bu metodu değiştirmek
    }
}
```

### Sorunlar:

1. **Open/Closed Principle ihlali** → Yeni yöntem eklemek için `PaymentService` değişmeli
2. **Single Responsibility ihlali** → Tek metod tüm algoritmaları barındırıyor
3. **Testleme zorluğu** → Bir yöntemi test etmek için tüm sınıfı yüklemek gerekir
4. **Kod şişmesi** → 10 ödeme yöntemi = devasa if/else bloğu
5. **Runtime esnekliği yok** → Algoritma değiştirilemez, sabit hardcoded

---

## ✅ ÇÖZÜM: Strategy Pattern

### Felsefe: "Algoritmayı kapsülle, değiştirilebilir yap"

```
Context (PaymentService)
└── IPaymentStrategy (interface)
    ├── CreditCardPayment
    ├── PayPalPayment
    └── CryptoPayment
```

### Üç Adım:

**1. Strategy Interface tanımla:**
```csharp
public interface IPaymentStrategy
{
    void Pay(decimal amount);
    bool Validate();
}
```

**2. Her algoritma kendi sınıfında:**
```csharp
public class CreditCardPayment : IPaymentStrategy { ... }
public class PayPalPayment    : IPaymentStrategy { ... }
public class CryptoPayment    : IPaymentStrategy { ... }
```

**3. Context sadece interface'i bilir:**
```csharp
public class PaymentService
{
    private IPaymentStrategy _strategy;

    // ✅ Runtime'da algoritma değiştirilebilir
    public void SetStrategy(IPaymentStrategy strategy)
        => _strategy = strategy;

    public void ProcessPayment(decimal amount)
    {
        if (_strategy.Validate())
            _strategy.Pay(amount);
    }
}
```

### Kullanım:
```csharp
var service = new PaymentService();

// Kredi kartı ile öde
service.SetStrategy(new CreditCardPayment("4242...", "123", "12/26"));
service.ProcessPayment(299.99m);

// Runtime'da değiştir
service.SetStrategy(new PayPalPayment("user@mail.com", "pass"));
service.ProcessPayment(149.00m);
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | STRATEGY |
|---------|---------|----------|
| **Yeni yöntem eklemek** | Kodu değiştir ❌ | Yeni sınıf ekle ✅ |
| **Test izolasyonu** | Zor ❌ | Kolay ✅ |
| **Runtime değişim** | Yok ❌ | Var ✅ |
| **Kod boyutu** | Şişiyor ❌ | Sabit ✅ |
| **Open/Closed** | İhlal ❌ | Sağlı ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 💳 **Ödeme sistemleri** ← Örneğimiz
- 🗜️ **Sıkıştırma algoritmaları** — ZIP, GZIP, BZIP
- 🔐 **Şifreleme** — AES, RSA, DES seçimi
- 📦 **Kargo hesaplama** — DHL, FedEx, PTT
- 🎮 **Oyun AI** — Saldırgan, savunmacı, pasif strateji
- 📊 **Sıralama algoritmaları** — QuickSort, MergeSort, BubbleSort

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
