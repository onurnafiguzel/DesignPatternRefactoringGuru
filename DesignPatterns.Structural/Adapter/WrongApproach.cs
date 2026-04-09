namespace DesignPatterns.Structural.Adapter.WrongApproach;

/// <summary>
/// ❌ KÖTÜ YAKLAŞIM: Uyumsuz interface'leri zorla yönet
///
/// Sorun: Email ve SMS için farklı interface'ler var.
/// Bunları aynı yerde yönetmek istiyoruz ama nasıl?
/// Cevap: if/else'ler ve tight coupling!
/// </summary>

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

// Email implementasyonu
public class SmtpEmailService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"📧 E-posta gönderiliyor");
        Console.WriteLine($"   Kime: {to}");
        Console.WriteLine($"   Konu: {subject}");
        Console.WriteLine($"   İçerik: {body}");
    }
}

// SMS implementasyonu
public class TwilioSmsGateway : ISmsGateway
{
    public void SendSms(string phoneNumber, string message)
    {
        Console.WriteLine($"📱 SMS gönderiliyor");
        Console.WriteLine($"   Numaraya: {phoneNumber}");
        Console.WriteLine($"   Mesaj: {message}");
    }
}

/// <summary>
/// ❌ PROBLEM 1: Tight Coupling
/// NotificationService tüm interface'leri bilmeli
/// </summary>
public class NotificationService
{
    private IEmailService? _emailService;
    private ISmsGateway? _smsGateway;

    public NotificationService(IEmailService emailService, ISmsGateway? smsGateway = null)
    {
        _emailService = emailService;
        _smsGateway = smsGateway;
    }

    /// <summary>
    /// ❌ PROBLEM 2: Type check'ler ve if/else
    /// </summary>
    public void SendNotification(string contact, string message, string type)
    {
        if (type == "email")
        {
            // Email parametreleri: to, subject, body
            _emailService?.SendEmail(contact, "Bildirim", message);
        }
        else if (type == "sms")
        {
            // SMS parametreleri: phoneNumber, message
            // ❌ contact'ı phone number, message'ı direkt mesaj olarak kullan
            // ❌ Subject'in nereye gideceği belirsiz!
            _smsGateway?.SendSms(contact, message);
        }
        else
        {
            Console.WriteLine("❌ Bilinmeyen notification tipi!");
        }
    }
}

/// <summary>
/// ❌ PROBLEM 3: Yeni tür eklemek gerekirse?
/// Push Notification istiyoruz!
/// </summary>
public interface IPushNotificationService
{
    void SendPush(string deviceId, string title, string content);
}

public class FirebaseCloud : IPushNotificationService
{
    public void SendPush(string deviceId, string title, string content)
    {
        Console.WriteLine($"🔔 Push Notification gönderiliyor");
        Console.WriteLine($"   Cihaza: {deviceId}");
        Console.WriteLine($"   Başlık: {title}");
        Console.WriteLine($"   İçerik: {content}");
    }
}

/// <summary>
/// ❌ PROBLEM 4: NotificationService'e yeni if/else eklemek gerekir
/// </summary>
public class ExtendedNotificationService
{
    private IEmailService? _emailService;
    private ISmsGateway? _smsGateway;
    private IPushNotificationService? _pushService;

    public ExtendedNotificationService(
        IEmailService emailService,
        ISmsGateway? smsGateway = null,
        IPushNotificationService? pushService = null)
    {
        _emailService = emailService;
        _smsGateway = smsGateway;
        _pushService = pushService;
    }

    /// <summary>
    /// ❌ if/else zinciri uzuyor, karmaşıklaşıyor
    /// </summary>
    public void SendNotification(string contact, string message, string type, string? extra = null)
    {
        if (type == "email")
        {
            _emailService?.SendEmail(contact, "Bildirim", message);
        }
        else if (type == "sms")
        {
            _smsGateway?.SendSms(contact, message);
        }
        else if (type == "push")  // ❌ YENİ KOD
        {
            // ❌ contact'ı deviceId, extra'yı title gibi kullan?
            // ❌ Semantik karmaşıklığı arttı
            _pushService?.SendPush(contact, extra ?? "Başlık", message);
        }
        else if (type == "slack")  // ❌ DİĞER YENİ KOD
        {
            // ❌ Slack için başka interface, başka parametreler...
        }
        else
        {
            Console.WriteLine("❌ Bilinmeyen notification tipi!");
        }
    }
}

public class WrongApproachDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║    ❌ ADAPTER PATTERN OLMADAN (Kötü Yaklaşım)            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        var emailService = new SmtpEmailService();
        var smsGateway = new TwilioSmsGateway();
        var pushService = new FirebaseCloud();

        var notifier = new ExtendedNotificationService(emailService, smsGateway, pushService);

        Console.WriteLine("--- Çeşitli bildirimler gönderiliyor ---\n");

        // Email gönder
        Console.WriteLine("1. Email göndergisi:");
        notifier.SendNotification("user@example.com", "Hoşgeldiniz sisteme", "email");

        Console.WriteLine();

        // SMS gönder
        Console.WriteLine("2. SMS göndergisi:");
        notifier.SendNotification("+905551234567", "Hoşgeldiniz sisteme", "sms");

        Console.WriteLine();

        // Push gönder
        Console.WriteLine("3. Push göndergisi:");
        notifier.SendNotification("device123", "Hoşgeldiniz", "push", "Sisteme Kaydınız Tamamlandı");

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("SORUNLAR:");
        Console.WriteLine("1. ❌ Interface uyumsuzluğu (Email: 3 param, SMS: 2 param)");
        Console.WriteLine("2. ❌ Type check'ler ve if/else zinciri");
        Console.WriteLine("3. ❌ ExtendedNotificationService'i her yeni tür için değiş");
        Console.WriteLine("4. ❌ Tight coupling (tüm detayları bilmesi gerekir)");
        Console.WriteLine("5. ❌ Testlemesi zor (mock yapısı karmaşık)");
        Console.WriteLine("6. ❌ Open/Closed Principle ihlali");
        Console.WriteLine("7. ❌ Semantic sorunlar (hangi param neye?)\n");
    }
}
