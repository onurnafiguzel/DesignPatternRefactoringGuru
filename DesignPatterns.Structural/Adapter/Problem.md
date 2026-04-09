# Adapter Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

E-commerce uygulamanızda müşterilere **Email** göndermek için `IEmailService` arayüzü kullanılıyor. Şimdi **SMS** de göndermek istiyorsunuz, ama SMS provider (örn. Twilio) tamamen farklı bir API sunar.

**Gereksinimler:**
- Varolan `IEmailService` interface'ini değiştirmemek
- Varolan Email göndergisi kodunu değiştirmemek
- SMS provider'ını Email provider'ı gibi kullanmak
- İleride başka notification türleri eklemek kolay olsun

---

## ❌ PROBLEM: Pattern Olmadan

Bakın: [WrongApproach.cs](WrongApproach.cs)

### Sorun 1: İnterface Uyumsuzluğu

```csharp
// Varolan Email interface
public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

// Yeni SMS interface (tamamen farklı)
public interface ISmsGateway
{
    void SendSms(string phoneNumber, string message);
}

// PROBLEM: Parametreler farklı!
// - Email: to, subject, body (3 param)
// - SMS: phoneNumber, message (2 param)
```

### Sorun 2: Kod Karmaşıklığı

```csharp
public class NotificationService
{
    public void SendNotification(string contact, string message, string type)
    {
        if (type == "email")
        {
            _emailService.SendEmail(contact, "Subject", message);
        }
        else if (type == "sms")
        {
            // ❌ ISmsGateway ve IEmailService uyumsuz
            // Type check'ler, if/else'ler çoğalacak
            // İleride "push", "slack" eklenirse?
        }
        else if (type == "push")
        {
            // ❌ Başka interface, başka yönetim
        }
    }
}
```

### Sorun 3: Tight Coupling

```csharp
// Her notifikasyon türünün detaylarını bilmeliyiz
public void SendNotification(string contact, string message, string type)
{
    switch (type)
    {
        case "email":
            // Email-specific logic
            break;
        case "sms":
            // SMS-specific logic
            break;
        case "slack":
            // Slack-specific logic
            break;
    }
}

// ❌ NotificationService'i her yeni tür için değiştirmek zorundayız
// ❌ Open/Closed Principle ihlali
```

### Sorun 4: Bakım Zorluğu

```csharp
// Yeni bir channel eklemek istiyoruz (Push Notification)

public interface IPushNotificationService
{
    void SendPush(string deviceId, string title, string content);
}

// ❌ Şimdi üç farklı interface var
// ❌ NotificationService'e yeni if/else eklemek gerekir
// ❌ Her seferinde kod değiştirilmeli

public void SendNotification(string contact, string message, string type)
{
    if (type == "email") { ... }
    else if (type == "sms") { ... }
    else if (type == "push") { ... }  // ✗ YENİ KOD EKLE
}
```

---

## ✅ ÇÖZÜM: Adapter Pattern

Bakın: [Pattern.cs](Pattern.cs)

### Felsefe: "Uyumsuz interface'i aynı interface'e dönüştür"

```csharp
// Varolan interface kalır (değiştirilmez)
public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

// Yeni SMS interface
public interface ISmsGateway
{
    void SendSms(string phoneNumber, string message);
}

// ✅ ADAPTER: ISmsGateway'i IEmailService gibi kullan
public class SmsToEmailAdapter : IEmailService
{
    private readonly ISmsGateway _smsGateway;
    
    public SmsToEmailAdapter(ISmsGateway smsGateway)
    {
        _smsGateway = smsGateway;
    }
    
    // ✅ ISmsGateway metodunu IEmailService interface'ine "çevir"
    public void SendEmail(string to, string subject, string body)
    {
        // "to" parametresini phone number gibi kullan
        // "subject" + "body"'i SMS mesajına dönüştür
        var message = $"{subject}: {body}";
        _smsGateway.SendSms(to, message);  // ✅ Çevirme tamamlandı
    }
}

// Kullanım:
var emailService = new SmtpEmailService();
var smsGateway = new TwilioSmsGateway();
var smsAsEmail = new SmsToEmailAdapter(smsGateway);

// Aynı interface ile ikisini de kullan!
emailService.SendEmail("user@example.com", "Hoş", "geldiniz");
smsAsEmail.SendEmail("+905551234567", "Hoş", "geldiniz");  // ✅ Aynı metod!
```

### Avantajlar:

✅ **IEmailService interface'i sabit kaldı** — Varolan code'u değiştirmedik  
✅ **ISmsGateway'i adaptör ettik** — Başka bir sınıf ile sarındı  
✅ **Single Responsibility** — Adapter sadece "çeviri" yapar  
✅ **Open/Closed Principle** — Yeni SMS adapter'lar ekleyebiliriz  
✅ **Reusability** — SMS adapter'ını başka yerlerde de kullanabiliriz  

---

## 📊 Karşılaştırma

### Pattern OLMADAN:

```csharp
// NotificationService'de tüm detaylar
public void SendNotification(string to, string msg, string type)
{
    switch (type)
    {
        case "email":
            _emailService.SendEmail(to, "Subj", msg);
            break;
        case "sms":
            _smsGateway.SendSms(to, msg);  // ❌ Parametreler farklı
            break;
        case "push":
            _pushService.SendPush(to, "Title", msg);  // ❌ Başka parametre
            break;
    }
}
```

**Sorunlar:**
- 😞 Karmaşık if/else
- 😞 Tight coupling
- 😞 Her yeni tür için değişim gerekir
- 😞 Testleme zor

### Pattern İLE:

```csharp
// Tüm şeyler aynı interface'e uydu!
public void SendNotification(string to, string subject, string message)
{
    // Hangi adapter kullanırsanız kullanın, aynı metodlar
    _notificationService.SendEmail(to, subject, message);
}

// Email
var emailService = new SmtpEmailService();

// SMS (adapter ile)
var smsAdapter = new SmsToEmailAdapter(new TwilioSmsGateway());

// Push (adapter ile)
var pushAdapter = new PushToEmailAdapter(new FirebaseCloud());

// Tümü IEmailService interface'i ile çalışır!
```

**Avantajlar:**
- ✅ Clean code
- ✅ Loose coupling
- ✅ Kolayca yeni adapter'lar eklenir
- ✅ Testlemesi kolay

---

## 🔧 İmplementasyon Detayları

### 1. Target Interface (Varolan)
```csharp
public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}
```

### 2. Adaptee Interface (Yeni, uyumlu olmayan)
```csharp
public interface ISmsGateway
{
    void SendSms(string phoneNumber, string message);
}
```

### 3. Adapter (Çevirmen)
```csharp
public class SmsToEmailAdapter : IEmailService
{
    private readonly ISmsGateway _smsGateway;
    
    public SmsToEmailAdapter(ISmsGateway smsGateway)
    {
        _smsGateway = smsGateway;
    }
    
    public void SendEmail(string to, string subject, string body)
    {
        // Çeviri yap
        var message = $"{subject}: {body}";
        _smsGateway.SendSms(to, message);
    }
}
```

---

## 💡 Ne Zaman Kullanılır?

- 📚 **Third-party Library Integration** — Uyumsuz kütüphaneler kullanırken
- 🏢 **Legacy System Modernization** — Eski sistemleri yeni kodla birleştirme
- 📱 **Multi-provider Support** — Email, SMS, Push, Slack gibi çoklu kanallar
- 🔌 **Hardware Integration** — Farklı donanım API'leri
- 📊 **Data Format Conversion** — XML ↔ JSON gibi dönüşümler

---

## ⚠️ Dikkat!

1. **Adapter Zincirleme** — Çok fazla adapter katmanı karmaşık yapabilir
2. **Tamamen Farklı Semantik** → Adapter anlamlı olmayabilir
3. **Alternative: Strategy** → Çok farklı işler yapıyorsa Strategy tercih edin

Bakın: [Example.cs](Example.cs) — Tam çalışan örnek
