# Proxy Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/proxy](https://refactoring.guru/design-patterns/proxy) adresindeki senaryodan esinlenmiştir.

Bir ödeme altyapısı geliştiriyorsunuz. Gerçek ödeme servisi (`RealPaymentGateway`) dış bir banka API'siyle konuşuyor. Her çağrı maliyetli:

- Ağ gecikmesi: ~300ms
- İşlem başına maliyet var
- Fraud kontrolü gerekiyor
- Her işlem loglanmalı
- Yetkisiz kullanıcı erişmemeli

Tüm bu kontrolleri **gerçek gateway'e gömmek** istemiyoruz — o sadece ödeme alsın.

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class RealPaymentGateway
{
    public ChargeResult Charge(string cardToken, decimal amount, string userId)
    {
        // ❌ İş mantığı burada olmamalı ama başka yer yok
        if (!IsAuthorized(userId))
            throw new UnauthorizedAccessException();

        if (IsSuspiciousAmount(amount))
        {
            LogFraudAttempt(userId, amount);
            throw new FraudDetectedException();
        }

        Log($"Charge attempt: {userId} {amount}");

        // Gerçek banka API çağrısı
        var result = BankApi.ProcessPayment(cardToken, amount);

        Log($"Charge result: {result.TransactionId}");
        return result;
    }

    // ❌ Gateway hem ödeme yapıyor hem fraud kontrolü hem loglama
    // ❌ Single Responsibility ihlali
    // ❌ Test için gerçek banka API'si çağrısı gerekiyor
    // ❌ Log, fraud, auth değiştirince gateway değişmeli
}
```

### Sorunlar:

1. **Single Responsibility ihlali** → Gateway ödeme + fraud + log + auth yapıyor
2. **Test zorluğu** → Gerçek banka API'si olmadan test edilemiyor
3. **Cross-cutting concern karmaşası** → Auth, log, cache her yere yayılıyor
4. **Değişim riski** → Fraud kuralı değişince gateway kodu değişmeli
5. **Pahalı kaynak kontrolsüz** → Her çağrı direkt bankaya gidiyor

---

## ✅ ÇÖZÜM: Proxy Pattern

### Felsefe: "Gerçek nesneyle aynı interface'i uygula, araya gir"

```
IPaymentGateway
├── RealPaymentGateway     ← Sadece ödeme alır, başka bilmez
└── PaymentGatewayProxy    ← Gerçeğin önünde durur
    ├── Auth kontrolü
    ├── Fraud tespiti
    ├── Rate limiting
    ├── Loglama
    └── (tüm kontroller geçince) → RealPaymentGateway.Charge()
```

### Proxy Türleri:

| Tür | Ne yapar? |
|-----|-----------|
| **Protection Proxy** | Erişim / yetki kontrolü ← Bu örnek |
| **Virtual Proxy** | Lazy initialization (pahalı nesneyi geç oluştur) |
| **Caching Proxy** | Tekrar eden sonuçları önbellekle |
| **Logging Proxy** | Her çağrıyı logla |
| **Remote Proxy** | Farklı makinedeki nesneye local gibi eriş |

### Kullanım:
```csharp
// İstemci kod farkı görmez — aynı interface
IPaymentGateway gateway = new PaymentGatewayProxy(new RealPaymentGateway());

gateway.Charge("tok_visa", 250m, "user-001");
// 1. Proxy: Auth kontrolü
// 2. Proxy: Fraud kontrolü
// 3. Proxy: Rate limit kontrolü
// 4. Proxy: Log (öncesi)
// 5. RealGateway: Ödeme al
// 6. Proxy: Log (sonrası)
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | PROXY |
|---------|---------|-------|
| **Single Responsibility** | İhlal ❌ | Her sınıf tek iş ✅ |
| **Test** | Gerçek API gerekir ❌ | Mock gateway ✅ |
| **Cross-cutting concern** | Dağınık ❌ | Proxy'de merkezi ✅ |
| **Gerçek gateway değişimi** | Kontroller etkilenir ❌ | Sadece proxy ✅ |
| **Yeni kontrol ekleme** | Gateway değişir ❌ | Proxy değişir ✅ |

---

## Proxy vs Decorator

| | Decorator | Proxy |
|---|---|---|
| **Amaç** | Davranış *ekle* | Erişimi *kontrol et* |
| **İstemci bilgisi** | Genelde bilir | Genelde bilmez |
| **Nesne ömrü** | İstemci oluşturur | Proxy yönetir |
| **Sarma sayısı** | Çok katman | Genelde 1 |

---

## 💡 Ne Zaman Kullanılır?

- 💳 **Ödeme gateway koruması** ← Bu örnek
- 🔒 **ACL / Yetki kontrolü** — Kullanıcı bu kaynağa erişebilir mi?
- 💾 **Lazy loading** — ORM'lerin `virtual` navigation property'leri
- ⚡ **Caching proxy** — Pahalı DB sorguları için önbellek
- 📡 **Remote proxy** — gRPC stub, WCF proxy, REST client wrapper
- 🚦 **Rate limiting** — API call throttling

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
