# Flyweight Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/flyweight](https://refactoring.guru/design-patterns/flyweight) adresindeki senaryodan esinlenmiştir.

Bir ödeme işleme sistemi geliştiriyorsunuz. Günlük **milyonlarca işlem** oluşturuluyor ve bunları bellekte tutmanız gerekiyor (gerçek zamanlı raporlama için). Her `Transaction` nesnesinin içinde tekrarlayan veriler var:

```
Transaction (her biri için):
  ├── transactionId    → benzersiz     (unique)
  ├── amount           → benzersiz     (unique)
  ├── timestamp        → benzersiz     (unique)
  ├── currency         → tekrarlar     "TRY" / "USD" / "EUR"
  ├── merchantCategory → tekrarlar     "FOOD" / "FUEL" / "TECH"
  ├── processorName    → tekrarlar     "Visa" / "Mastercard" / "Troy"
  └── processorFees    → tekrarlar     {interchangeFee, schemesFee, ...}
```

**1.000.000 işlem × tekrarlayan veri boyutu = ciddi bellek problemi.**

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class Transaction
{
    // Benzersiz veriler
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }

    // ❌ Tekrarlayan veriler: her nesnede kopyalanıyor
    public string Currency { get; set; }          // "TRY" — 1M kez
    public string MerchantCategory { get; set; }  // "FOOD" — 1M kez
    public string ProcessorName { get; set; }     // "Visa" — 1M kez
    public decimal InterchangeFee { get; set; }   // 0.015m — 1M kez
    public decimal SchemesFee { get; set; }       // 0.003m — 1M kez
    public string ProcessorLogoUrl { get; set; }  // uzun string — 1M kez
}

// 1.000.000 Transaction nesnesi = tüm bu veriler 1M kez bellekte
// ❌ Gereksiz bellek tüketimi
// ❌ GC baskısı artar
```

### Bellek Analizi (1M İşlem):

| Alan | Boyut | Toplam |
|------|-------|--------|
| ProcessorName | ~20 byte | 20 MB |
| ProcessorLogoUrl | ~100 byte | 100 MB |
| InterchangeFee | 8 byte | 8 MB |
| MerchantCategory | ~15 byte | 15 MB |
| **Tekrarlayan toplam** | ~143 byte | **~143 MB** |

Oysa kaç farklı değer var? Processor: 5, Category: 12, Currency: 8 — **toplam ~25 benzersiz kombinasyon**.

---

## ✅ ÇÖZÜM: Flyweight Pattern

### Felsefe: "Paylaşılabilir veriyi bir kez sakla, ihtiyaç duyanlara referans ver"

```
TransactionType (Flyweight — paylaşılan, değişmez)
├── currency
├── merchantCategory
├── processorName
├── interchangeFee
└── processorLogoUrl

Transaction (Context — benzersiz, hafif)
├── transactionId   ← unique
├── amount          ← unique
├── timestamp       ← unique
└── type            ← TransactionType referansı (sadece pointer!)
```

### Bellek Tasarrufu:

| | OLMADAN | FLYWEIGHT |
|---|---|---|
| **1M işlem** | 143 MB (tekrarlayan) | ~25 × birkaç KB |
| **Nesne sayısı** | 1M büyük nesne | 1M küçük + ~25 flyweight |

---

## 💡 Ne Zaman Kullanılır?

- 💳 **Ödeme işlem kayıtları** ← Bu örnek
- 🎮 **Oyun partikülleri** — Renk/doku paylaşımı (refactoring.guru örneği)
- 🌳 **Orman/harita** — Ağaç türleri paylaşımlı, konum benzersiz
- 📝 **Text editor** — Karakter biçimlendirme (font/renk paylaşımlı)
- 🗺️ **Harita pin'leri** — Icon/kategori paylaşımlı, koordinat benzersiz
- 📦 **Ürün kataloğu** — Kategori meta verisi paylaşımlı

## ⚠️ Ne Zaman Kullanılmaz?

- Nesne sayısı az ise (gereksiz karmaşıklık)
- Paylaşılan veri küçük/az ise (kazanç yok)
- Flyweight nesneleri sık değişiyorsa (thread-safety sorunu)

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
