# Bridge Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/bridge](https://refactoring.guru/design-patterns/bridge) adresindeki senaryodan esinlenmiştir.

Bir ödeme altyapısı geliştiriyorsunuz. İki bağımsız boyutunuz var:

**Boyut 1 — Ödeme Türü** (nasıl faturalandırılır?)
- Tek Seferlik Ödeme
- Taksitli Ödeme
- Abonelik (Recurring)

**Boyut 2 — Ödeme Kanalı** (nereden tahsil edilir?)
- Kredi Kartı
- Banka Transferi
- Dijital Cüzdan (Papara, PayPal)

Her kombinasyon için sipariş işlenebilmeli: **3 tür × 3 kanal = 9 senaryo**.

---

## ❌ PROBLEM: Pattern Olmadan

### Kalıtımla çözmek → Alt sınıf patlaması

```csharp
public abstract class Payment { }

// Tür × Kanal kombinasyonları için ayrı sınıf
public class OneTimeCreditCardPayment    : Payment { }
public class OneTimeBankTransferPayment  : Payment { }
public class OneTimeWalletPayment        : Payment { }

public class InstallmentCreditCardPayment   : Payment { }
public class InstallmentBankTransferPayment : Payment { }
public class InstallmentWalletPayment       : Payment { }

public class RecurringCreditCardPayment    : Payment { }
public class RecurringBankTransferPayment  : Payment { }
public class RecurringWalletPayment        : Payment { }

// ❌ 3 tür × 3 kanal = 9 sınıf
// ❌ 4. bir kanal (kripto) gelirse: +3 sınıf daha
// ❌ 4. bir tür (ertelenmiş) gelirse: +4 sınıf daha
// ❌ N tür × M kanal = N×M sınıf
```

### Sorunlar:

1. **Kombinatorik patlama** → N tür × M kanal = N×M alt sınıf
2. **Kod tekrarı** → Taksitli mantığı 3 farklı sınıfa kopyalanıyor
3. **Bağımsız değişim yok** → Kredi kartı mantığı değişirse 3 sınıfı güncelle
4. **Yeni boyut eklemek** → Tüm hiyerarşiyi yeniden düzenle

---

## ✅ ÇÖZÜM: Bridge Pattern

### Felsefe: "İki boyutu birbirinden ayır, köprüyle bağla"

```
Abstraction (Ödeme Türü)       Implementation (Ödeme Kanalı)
──────────────────────         ──────────────────────────────
Payment                        IPaymentChannel
├── OneTimePayment      ──────▶ ├── CreditCardChannel
├── InstallmentPayment  ──────▶ ├── BankTransferChannel
└── RecurringPayment    ──────▶ └── WalletChannel
         │                              │
         └──────── bridge ──────────────┘
                  (composition)
```

Her `Payment` bir `IPaymentChannel` referansı taşır (composition). İki boyut **birbirinden bağımsız genişler**.

### Kullanım:
```csharp
IPaymentChannel card     = new CreditCardChannel("4242…", "123");
IPaymentChannel transfer = new BankTransferChannel("TR33…");

// Aynı kanal, farklı tür
new OneTimePayment(card).Process(500m);
new InstallmentPayment(card, installments: 6).Process(500m);
new RecurringPayment(card, interval: "monthly").Process(500m);

// Aynı tür, farklı kanal — KANAL KODU HİÇ DEĞİŞMEDİ
new OneTimePayment(transfer).Process(500m);
```

---

## Bridge vs Adapter

| | Adapter | Bridge |
|---|---|---|
| **Tasarım zamanı** | Sonradan (uyumsuzluk var) | Önceden (büyümeyi öngör) |
| **Amaç** | Uyumsuzluğu gider | İki boyutu ayır |
| **Boyut sayısı** | 1 (interface çevirisi) | 2+ (bağımsız hiyerarşi) |

## 📊 Karşılaştırma

| Özellik | OLMADAN | BRIDGE |
|---------|---------|--------|
| **Sınıf sayısı (3×3)** | 9 sınıf ❌ | 3+3=6 sınıf ✅ |
| **Yeni kanal ekleme** | +N sınıf ❌ | +1 sınıf ✅ |
| **Yeni tür ekleme** | +M sınıf ❌ | +1 sınıf ✅ |
| **Kanal değişimi** | N sınıfı güncelle ❌ | 1 sınıfı güncelle ✅ |
| **Runtime kanal değiştirme** | İmkansız ❌ | Mümkün ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 💳 **Ödeme sistemi** ← Bu örnek (tür × kanal)
- 🖨️ **Printer sistemi** — Belge türü × yazıcı markası
- 🌐 **Mesajlaşma** — Mesaj türü (bildirim/uyarı) × kanal (email/sms)
- 🎮 **Oyun** — Silah türü × platform (PC/konsol/mobil)
- 📱 **UI** — Widget türü × platform teması

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
