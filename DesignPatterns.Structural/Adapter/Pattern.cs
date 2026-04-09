namespace DesignPatterns.Structural.Adapter.Correct;

/// <summary>
/// ✅ ADAPTER PATTERN: Uyumsuz interface'leri uyumlu hale getir
///
/// Amaç:
/// - İki uyumsuz interface'i bir arada çalışabilir hale getir
/// - Varolan code'u değiştirmeden yeni functionality ekle
/// - "Çevirmen" sınıf aracılığıyla uyumu sağla
///
/// Kullanım Alanları:
/// - Third-party library integration
/// - Email, SMS, Push gibi multi-channel notification
/// - Legacy system modernization
/// - Hardware/Driver integration
/// - Data format conversion (XML, JSON, CSV)
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Target Interface (Varolan, değiştirilmeyecek)
// ═══════════════════════════════════════════════════════════

public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Adapter Interfaces (Yeni, uyumlu olmayan)
// ═══════════════════════════════════════════════════════════

public interface ISmsGateway
{
    void SendSms(string phoneNumber, string message);
}

public interface IPushNotificationService
{
    void SendPush(string deviceId, string title, string content);
}

public interface ISlackNotificationService
{
    void SendToChannel(string channelName, string message);
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Concrete Implementations
// ═══════════════════════════════════════════════════════════

// Email (varolan)
public class SmtpEmailService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"   📧 E-posta: {to}");
        Console.WriteLine($"      Konu: {subject}");
        Console.WriteLine($"      İçerik: {body}");
    }
}

// SMS (yeni)
public class TwilioSmsGateway : ISmsGateway
{
    public void SendSms(string phoneNumber, string message)
    {
        Console.WriteLine($"   📱 SMS: {phoneNumber}");
        Console.WriteLine($"      Mesaj: {message}");
    }
}

// Push (yeni)
public class FirebaseCloud : IPushNotificationService
{
    public void SendPush(string deviceId, string title, string content)
    {
        Console.WriteLine($"   🔔 Push: {deviceId}");
        Console.WriteLine($"      Başlık: {title}");
        Console.WriteLine($"      İçerik: {content}");
    }
}

// Slack (yeni)
public class SlackWebhook : ISlackNotificationService
{
    public void SendToChannel(string channelName, string message)
    {
        Console.WriteLine($"   💬 Slack: #{channelName}");
        Console.WriteLine($"      Mesaj: {message}");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 4: ADAPTERS (Uyumlu hale getiriciler)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ ADAPTER 1: ISmsGateway'i IEmailService gibi kullan
/// Çevirme kuralları:
/// - SMS "to" = Email "to"
/// - SMS "message" = Email "subject + body"
/// </summary>
public class SmsToEmailAdapter : IEmailService
{
    private readonly ISmsGateway _smsGateway;

    public SmsToEmailAdapter(ISmsGateway smsGateway)
    {
        _smsGateway = smsGateway;
    }

    public void SendEmail(string to, string subject, string body)
    {
        // ✅ Çeviri: Email mesajını SMS formatına dönüştür
        var smsMessage = $"{subject}: {body}";
        _smsGateway.SendSms(to, smsMessage);
    }
}

/// <summary>
/// ✅ ADAPTER 2: IPushNotificationService'i IEmailService gibi kullan
/// Çevirme kuralları:
/// - Push "deviceId" = Email "to"
/// - Push "title" = Email "subject"
/// - Push "content" = Email "body"
/// </summary>
public class PushToEmailAdapter : IEmailService
{
    private readonly IPushNotificationService _pushService;

    public PushToEmailAdapter(IPushNotificationService pushService)
    {
        _pushService = pushService;
    }

    public void SendEmail(string to, string subject, string body)
    {
        // ✅ Çeviri: Email mesajını Push formatına dönüştür
        _pushService.SendPush(to, subject, body);
    }
}

/// <summary>
/// ✅ ADAPTER 3: ISlackNotificationService'i IEmailService gibi kullan
/// Çevirme kuralları:
/// - Slack "channel" = Email "to"
/// - Slack "message" = Email "subject + body"
/// </summary>
public class SlackToEmailAdapter : IEmailService
{
    private readonly ISlackNotificationService _slackService;

    public SlackToEmailAdapter(ISlackNotificationService slackService)
    {
        _slackService = slackService;
    }

    public void SendEmail(string to, string subject, string body)
    {
        // ✅ Çeviri: Email mesajını Slack formatına dönüştür
        var slackMessage = $"*{subject}*\n{body}";
        _slackService.SendToChannel(to, slackMessage);
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 5: Client Code (Tüm interface'ler aynı şekilde kullanılır)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ Clean NotificationService
/// Sadece IEmailService interface'i ile çalışır
/// Kaç farklı notification türü varsa önemli değil!
/// </summary>
public class NotificationService
{
    private readonly List<IEmailService> _notificationChannels;

    public NotificationService()
    {
        _notificationChannels = new List<IEmailService>();
    }

    public void AddNotificationChannel(IEmailService channel)
    {
        _notificationChannels.Add(channel);
    }

    /// <summary>
    /// ✅ Tüm kanal türlerini aynı şekilde kullan
    /// if/else yok, switch yok!
    /// </summary>
    public void SendNotificationToAll(string to, string subject, string body)
    {
        Console.WriteLine($"\n▶ Bildiri gönderiliyor ({_notificationChannels.Count} kanala)\n");

        foreach (var channel in _notificationChannels)
        {
            channel.SendEmail(to, subject, body);
        }
    }

    public void SendNotification(int channelIndex, string to, string subject, string body)
    {
        if (channelIndex >= 0 && channelIndex < _notificationChannels.Count)
        {
            Console.WriteLine();
            _notificationChannels[channelIndex].SendEmail(to, subject, body);
        }
    }
}

public class AdapterPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         ✅ ADAPTER PATTERN (Doğru Yaklaşım)             ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // ✅ Tüm service'ler oluşturuluyor
        var emailService = new SmtpEmailService();
        var smsGateway = new TwilioSmsGateway();
        var pushService = new FirebaseCloud();
        var slackService = new SlackWebhook();

        // ✅ ADAPTER'lar ile sarılıyor
        var smsAdapter = new SmsToEmailAdapter(smsGateway);
        var pushAdapter = new PushToEmailAdapter(pushService);
        var slackAdapter = new SlackToEmailAdapter(slackService);

        // ✅ NotificationService'e ekleniyor
        var notificationService = new NotificationService();
        notificationService.AddNotificationChannel(emailService);
        notificationService.AddNotificationChannel(smsAdapter);
        notificationService.AddNotificationChannel(pushAdapter);
        notificationService.AddNotificationChannel(slackAdapter);

        // ✅ Tümü aynı interface ile çalışıyor!
        Console.WriteLine("=== Tekil Bildirimler (Adapter Pattern) ===\n");

        notificationService.SendNotification(0, "user@example.com", "Hoşgeldiniz", "Sisteme başarıyla kaydoldunuz");
        notificationService.SendNotification(1, "+905551234567", "Hoşgeldiniz", "Sisteme başarıyla kaydoldunuz");
        notificationService.SendNotification(2, "device-12345", "Hoşgeldiniz", "Sisteme başarıyla kaydoldunuz");
        notificationService.SendNotification(3, "announcements", "Hoşgeldiniz", "Sisteme başarıyla kaydoldunuz");

        // ✅ Tüm kanallara bir kez gönder
        notificationService.SendNotificationToAll(
            "user@example.com",
            "Önemli Duyuru",
            "Yeni özellikler eklendi!");

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ Tüm interface'ler IEmailService gibi kullanılıyor");
        Console.WriteLine("✓ NotificationService sadece IEmailService biliyor");
        Console.WriteLine("✓ Varolan code'u değiştirmedik");
        Console.WriteLine("✓ Yeni adapter eklemek kolay");
        Console.WriteLine("✓ if/else yok, switch yok!");
        Console.WriteLine("✓ Open/Closed Principle ✓");
        Console.WriteLine("✓ Testlemesi kolay");
        Console.WriteLine("✓ Clean code\n");
    }
}
