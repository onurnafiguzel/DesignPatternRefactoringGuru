# Facade Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/facade](https://refactoring.guru/design-patterns/facade) adresindeki senaryodan esinlenmiştir.

Bir e-ticaret uygulamasında **sipariş işleme** geliştiriyorsunuz. Bir siparişin tamamlanması için birden fazla alt sistemin belirli sırada çalışması gerekiyor:

1. **Stok Servisi** — Ürün stokta var mı?
2. **Ödeme Servisi** — Ödeme onaylandı mı?
3. **Kargo Servisi** — Kargo oluştur, takip kodu al
4. **Fatura Servisi** — Fatura oluştur, müşteriye gönder
5. **Bildirim Servisi** — Müşteriye SMS/email gönder

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
// İstemci kod tüm alt sistemleri bilmek zorunda
public class OrderController
{
    public void PlaceOrder(int productId, int qty, string userId, string cardToken)
    {
        // ❌ Controller tüm alt sistemi koordine etmek zorunda
        var inventory = new InventoryService();
        if (!inventory.CheckStock(productId, qty))
            throw new Exception("Stok yetersiz");

        inventory.ReserveStock(productId, qty);

        var payment = new PaymentService();
        var charge = payment.Charge(cardToken, GetPrice(productId, qty));
        if (!charge.Success)
        {
            inventory.ReleaseReservation(productId, qty);  // ❌ rollback manuel
            throw new Exception("Ödeme başarısız");
        }

        var shipping = new ShippingService();
        var label = shipping.CreateLabel(userId, productId, qty);
        shipping.Dispatch(label.TrackingCode);

        var invoice = new InvoiceService();
        var inv = invoice.Generate(userId, productId, qty, charge.Amount);
        invoice.Send(inv, userId);

        var notification = new NotificationService();
        notification.SendOrderConfirmation(userId, label.TrackingCode);

        // ❌ Bu akış her sipariş noktasında tekrar yazılacak!
        // ❌ Controller 5 farklı servisi biliyor
        // ❌ Hata yönetimi ve rollback her yerde tekrar
    }
}
```

### Sorunlar:

1. **İstemci alt sistemleri bilmek zorunda** → Controller 5 servisin API'sini ezberlemeli
2. **Aynı akış her yerde tekrar** → DRY ihlali (mobil API, web API, admin paneli hepsi aynı kodu yazar)
3. **Sıra ve rollback yönetimi istemcide** → Karmaşık, hata yapmaya açık
4. **Alt sistem değişirse** → Tüm istemci kodlarını güncelle
5. **Test zorluğu** → Controller testi için 5 servisi birden mock'lamak gerekir

---

## ✅ ÇÖZÜM: Facade Pattern

### Felsefe: "Karmaşık alt sistemi basit bir arayüzün arkasına sakla"

```
OrderFacade
└── PlaceOrder(productId, qty, userId, cardToken)
    ├── InventoryService
    ├── PaymentService
    ├── ShippingService
    ├── InvoiceService
    └── NotificationService
```

### Kullanım:
```csharp
// ✅ İstemci sadece Facade'ı biliyor
var facade = new OrderFacade();
var result = facade.PlaceOrder(productId: 42, qty: 2, userId: "u1", cardToken: "tok_xxx");

// Alt sistemlerin hiçbirini bilmiyoruz
// Sıra, hata yönetimi, rollback → Facade'ın sorumluluğu
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | FACADE |
|---------|---------|--------|
| **İstemcinin bilmesi gereken** | 5 servis API'si ❌ | 1 Facade metodu ✅ |
| **Kod tekrarı** | Her istemcide aynı akış ❌ | Tek yerde ✅ |
| **Alt sistem değişimi** | Tüm istemciler etkilenir ❌ | Sadece Facade ✅ |
| **Test** | 5 mock gerekir ❌ | Sadece Facade mock ✅ |
| **Rollback yönetimi** | İstemcide ❌ | Facade'da ✅ |

---

## Facade vs Adapter

| | Adapter | Facade |
|---|---|---|
| **Amaç** | Interface dönüştür | Basitleştir / gizle |
| **Nesne sayısı** | Genelde 1 nesne | Alt sistem ailesi |
| **Interface** | Varolan'a uydur | Yeni, basit interface |

---

## 💡 Ne Zaman Kullanılır?

- 🛒 **Sipariş işleme** ← Örneğimiz
- 🎬 **Video dönüştürme** — codec, encoder, metadata (refactoring.guru örneği)
- 🔐 **Kimlik doğrulama** — token, session, permission, audit log
- 📦 **SDK / Kütüphane tasarımı** — Karmaşık iç yapıyı gizle, basit API sun
- 🏦 **Banka transferi** — bakiye kontrol, limit, fraud check, ledger, bildirim
- 🧪 **Test setup** — Karmaşık test ortamını tek çağrıyla hazırla

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
