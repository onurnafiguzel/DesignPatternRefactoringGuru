# Composite Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/composite](https://refactoring.guru/design-patterns/composite) adresindeki senaryodan esinlenmiştir.

Bir ödeme planı sistemi geliştiriyorsunuz. Müşteriler şu seçeneklere sahip:

- **Tekil Ödeme** → Tek bir ürün için ödeme (₺299)
- **Paket Ödeme** → Birden fazla ödemenin gruplanması (₺299 + ₺199 + ₺99)
- **İç İçe Paket** → Paket içinde paket (Yıllık Paket = [Aylık × 12] + Kurulum + Destek)

Fatura hesaplama, iptal ve özet görüntüleme **tekil için de, paket için de aynı şekilde** çalışmalı.

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class PaymentPlanService
{
    public decimal CalculateTotal(object plan)
    {
        // ❌ Tip kontrolü gerekiyor
        if (plan is SinglePayment single)
        {
            return single.Amount;
        }
        else if (plan is PaymentBundle bundle)
        {
            decimal total = 0;
            foreach (var item in bundle.Items)
            {
                // ❌ İç içe paket olursa? Recursive mi yapalım?
                if (item is SinglePayment s)
                    total += s.Amount;
                else if (item is PaymentBundle b)
                    total += CalculateBundleTotal(b); // ❌ Ayrı metod
            }
            return total;
        }
        throw new ArgumentException("Bilinmeyen plan tipi");
    }

    private decimal CalculateBundleTotal(PaymentBundle bundle) { ... }
    // ❌ İki farklı metod, aynı mantık, kod tekrarı
    // ❌ Yeni tip eklenince her yere if/else eklenir
}
```

### Sorunlar:

1. **Tip kontrolü her yerde** → `is SinglePayment`, `is PaymentBundle` if/else'leri
2. **Recursive iç içe yapı için ayrı metod** → Kod tekrarı
3. **Yeni plan tipi eklemek** → Tüm kontrol noktalarını güncelle
4. **İstemci ağaç yapısını bilmek zorunda** → Loose coupling yok
5. **Uniform işlem imkansız** → Tekil ve grup için ayrı kod yazılmalı

---

## ✅ ÇÖZÜM: Composite Pattern

### Felsefe: "Tekil ve grup aynı interface'i uygulasın, istemci farkı görmesin"

```
IPaymentComponent
├── Calculate() → decimal
├── Cancel()
└── Display(indent)

    ├── SinglePayment    ← Yaprak (Leaf)
    │   Calculate() → amount
    │
    └── PaymentBundle    ← Bileşik (Composite)
        ├── SinglePayment
        ├── SinglePayment
        └── PaymentBundle
            ├── SinglePayment
            └── SinglePayment
```

### Kullanım:
```csharp
// Tekil veya paket — aynı interface
IPaymentComponent basic   = new SinglePayment("Temel Plan", 299m);
IPaymentComponent support = new SinglePayment("Destek", 99m);

var bundle = new PaymentBundle("Başlangıç Paketi");
bundle.Add(basic);
bundle.Add(support);

// ✅ Aynı metod çağrısı — tekil mi paket mi bilmiyoruz
Console.WriteLine(bundle.Calculate());   // 398m
Console.WriteLine(basic.Calculate());    // 299m
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | COMPOSITE |
|---------|---------|-----------|
| **Tip kontrolü** | Her yerde ❌ | Yok ✅ |
| **İç içe yapı** | Ayrı recursive metod ❌ | Otomatik ✅ |
| **Yeni tip ekleme** | Her metodu güncelle ❌ | Interface ekle ✅ |
| **Uniform işlem** | İmkansız ❌ | Doğal ✅ |
| **Ağaç derinliği** | Hardcoded ❌ | Sonsuz ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 💳 **Ödeme planları** ← Bu örnek (tekil ve paket fatura)
- 📁 **Dosya sistemi** — Dosya ve klasör (refactoring.guru örneği)
- 🛒 **Alışveriş sepeti** — Tekil ürün ve ürün grupları
- 🏢 **Organizasyon şeması** — Çalışan ve departman
- 🖥️ **UI bileşenleri** — Widget ve container (HTML DOM tam olarak bu)
- 📦 **Kargo** — Tek parsel ve palet (iç içe kutular)

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
