# Chain of Responsibility Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

Bir müşteri destek sistemi geliştiriyorsunuz. Gelen destek taleplerinin (ticket) önce **otomatik bot**, sonra **1. seviye destek**, sonra **uzman**, en son **yönetici** tarafından ele alınması gerekiyor. Her seviye sadece kendi kapasitesindeki sorunları çözer, çözemezse bir üst seviyeye iletir.

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class SupportService
{
    public void HandleTicket(Ticket ticket)
    {
        // ❌ Tüm seviyeler tek metodda, iç içe koşullar
        if (ticket.Type == "faq")
        {
            Bot.Respond(ticket);
        }
        else if (ticket.Priority <= 2)
        {
            if (FirstLevel.CanHandle(ticket))
                FirstLevel.Handle(ticket);
            else if (Expert.CanHandle(ticket))
                Expert.Handle(ticket);
            else
                Manager.Handle(ticket);
        }
        else
        {
            Manager.Handle(ticket);
        }
        // ❌ Yeni seviye eklemek = bu bloğu değiştirmek
        // ❌ Hangi seviyenin ne işleyebileceği burada karar veriliyor
        // ❌ Seviyeler birbirini biliyor, coupling yüksek
    }
}
```

### Sorunlar:

1. **Yeni seviye eklemek** → `HandleTicket` metodunu değiştirmek gerekir
2. **Seviye sırası değiştirmek** → Koşul bloklarını yeniden yazmak
3. **Her seviye diğerini biliyor** → Tight coupling
4. **Karar merkezi tek noktada** → Single Responsibility ihlali
5. **Dinamik zincir yok** → Runtime'da sıra/seviye değiştirilemez

---

## ✅ ÇÖZÜM: Chain of Responsibility

### Felsefe: "Her halka işleyip işlemeyeceğine kendisi karar verir, işleyemezse bir sonrakine geçirir"

```
Ticket
  │
  ▼
[Bot Handler] ──── çözemedim ──▶ [1. Seviye] ──── çözemedim ──▶ [Uzman] ──── çözemedim ──▶ [Yönetici]
      │                               │                              │                           │
  çözdüm ✓                       çözdüm ✓                       çözdüm ✓                    çözdüm ✓
```

### Kullanım:
```csharp
// Zinciri kur (runtime'da değiştirilebilir)
var bot      = new BotHandler();
var level1   = new Level1Handler();
var expert   = new ExpertHandler();
var manager  = new ManagerHandler();

bot.SetNext(level1).SetNext(expert).SetNext(manager);

// Tüm ticketlar aynı noktadan girer
bot.Handle(new Ticket("Şifremi unuttum", Priority.Low));
bot.Handle(new Ticket("Fatura itirazı", Priority.High));
bot.Handle(new Ticket("Sistem hatası", Priority.Critical));
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | CHAIN OF RESP. |
|---------|---------|----------------|
| **Yeni seviye ekleme** | Merkezi kod değişir ❌ | Yeni halka ekle ✅ |
| **Seviye sırası** | Hardcoded ❌ | Runtime'da değişir ✅ |
| **Coupling** | Yüksek ❌ | Düşük ✅ |
| **Sorumluluk** | Tek sınıfta ❌ | Dağıtılmış ✅ |
| **Test** | Zor ❌ | Her halka ayrı ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 🎫 **Destek sistemleri** ← Örneğimiz (ticket escalation)
- 🔐 **Middleware / Pipeline** — ASP.NET Core middleware zinciri
- ✅ **Validation zinciri** — Alan doğrulamaları sırayla
- 🛡️ **Yetki / izin kontrolleri** — Role-based access
- 📋 **Log seviyeleri** — DEBUG → INFO → WARN → ERROR
- 💰 **Onay akışları** — Harcama limitlerine göre yönetici onayı

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
