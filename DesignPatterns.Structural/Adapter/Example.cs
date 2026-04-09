namespace DesignPatterns.Structural.Adapter.Examples;

using DesignPatterns.Structural.Adapter.Correct;
using DesignPatterns.Structural.Adapter.WrongApproach;

/// <summary>
/// Adapter Pattern'in karşılaştırmalı örneği
/// </summary>
public class AdapterComparison
{
    public static void RunAll()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("    ADAPTER PATTERN - KARŞILAŞTIRMALI ÖRNEK");
        Console.WriteLine(new string('═', 70));

        // Kötü yaklaşımı göster
        RunWrongApproach();

        // Doğru yaklaşımı göster
        RunCorrectApproach();

        // Karşılaştırma
        ShowComparison();
    }

    private static void RunWrongApproach()
    {
        WrongApproach.WrongApproachDemo.Run();

        Console.WriteLine("\nDevam etmek için Enter'a basınız...");
        Console.ReadLine();
    }

    private static void RunCorrectApproach()
    {
        Correct.AdapterPatternDemo.Run();

        Console.WriteLine("\nDevam etmek için Enter'a basınız...");
        Console.ReadLine();
    }

    private static void ShowComparison()
    {
        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("    KARŞILAŞTIRMA TABLOSU");
        Console.WriteLine(new string('═', 70) + "\n");

        var data = new[]
        {
            ("Özellik", "PATTERN OLMADAN", "ADAPTER PATTERN"),
            ("──────────────────────", "────────────────", "──────────────"),
            ("Interface'ler", "Uyumsuz ❌", "Uyumlu ✅"),
            ("Type Checking", "Çok ❌", "Yok ✅"),
            ("if/else Zinciri", "Uzun ❌", "Kısa ✅"),
            ("Yeni Tür Ekleme", "Değişim gerekli ❌", "Adapter ekle ✅"),
            ("Tight Coupling", "Yüksek ❌", "Düşük ✅"),
            ("Testlenebilirlik", "Zor ❌", "Kolay ✅"),
            ("Code Complexity", "Yüksek ❌", "Düşük ✅"),
            ("Open/Closed Principle", "Çiğneniyor ❌", "Saklı ✅"),
        };

        foreach (var (feature, wrong, correct) in data)
        {
            Console.WriteLine($"  {feature,-25} | {wrong,-20} | {correct,-20}");
        }

        Console.WriteLine("\n" + new string('═', 70));
    }
}

/// <summary>
/// Pratik senaryo: E-Commerce Notification Sistemi
/// </summary>
public class ECommerceNotificationExample
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     PRATIK SENARYO: E-Commerce Bildirim Sistemi         ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("Senaryo: Müşterilere Email, SMS, Push ve Slack'ten bildirim\n");

        // Servisler
        var emailService = new SmtpEmailService();
        var smsGateway = new TwilioSmsGateway();
        var pushService = new FirebaseCloud();
        var slackService = new SlackWebhook();

        // Adapter'lar
        var smsAdapter = new SmsToEmailAdapter(smsGateway);
        var pushAdapter = new PushToEmailAdapter(pushService);
        var slackAdapter = new SlackToEmailAdapter(slackService);

        // Notification Service
        var notificationService = new NotificationService();
        notificationService.AddNotificationChannel(emailService);
        notificationService.AddNotificationChannel(smsAdapter);
        notificationService.AddNotificationChannel(pushAdapter);
        notificationService.AddNotificationChannel(slackAdapter);

        // Senaryo 1: Yeni kullanıcı kaydı
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine("SENARYO 1: Yeni Kullanıcı Kaydı");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        notificationService.SendNotificationToAll(
            "user@example.com",
            "Hoş Geldiniz!",
            "Sisteme başarıyla kaydoldunuz. İlk siparişinizde %10 indirim alacaksınız!");

        // Senaryo 2: Sipariş onayı
        Console.WriteLine("\n═══════════════════════════════════════════════════════════");
        Console.WriteLine("SENARYO 2: Sipariş Onayı");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        notificationService.SendNotificationToAll(
            "user@example.com",
            "Siparişiniz Onaylandı",
            "Sipariş ID: #ORD-2025-001. Toplam: 299.99 TL. Kargo 2 iş günü içinde başlayacak.");

        // Senaryo 3: Kargo takibi
        Console.WriteLine("\n═══════════════════════════════════════════════════════════");
        Console.WriteLine("SENARYO 3: Kargo Çıkış Bildirimi (Farklı kanallar)");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine("\nEmail kanalına:\n");
        notificationService.SendNotification(0, "customer@email.com", "Kargo Çıkışı", "Siparişiniz kargoya verildi. Kargo Takip: TR123456789");

        Console.WriteLine("\nSMS kanalına:\n");
        notificationService.SendNotification(1, "+905551234567", "Kargo Çıkışı", "Siparişiniz kargoya verildi. Kargo Takip: TR123456789");

        Console.WriteLine("\nPush kanalına:\n");
        notificationService.SendNotification(2, "mobile-device-abc123", "Kargo Çıkışı", "Siparişiniz kargoya verildi. Kargo Takip: TR123456789");

        Console.WriteLine("\nSlack kanalına:\n");
        notificationService.SendNotification(3, "orders", "Kargo Çıkışı", "Siparişiniz kargoya verildi. Kargo Takip: TR123456789");

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("SONUÇ:");
        Console.WriteLine("✓ 4 farklı notification kanalı var");
        Console.WriteLine("✓ Hepsi IEmailService interface'i ile kullanılıyor");
        Console.WriteLine("✓ NotificationService hiçbir kanalın detaylarını bilmiyor");
        Console.WriteLine("✓ Yeni kanal eklemek için NotificationService'i değiştirmeye gerek yok");
        Console.WriteLine("✓ Adapter'lar çevirme işini yönetiyor\n");
    }
}

/// <summary>
/// Advanced Example: Dinamik Adapter Kaydı
/// </summary>
public class DynamicAdapterExample
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        ADVANCED: Dinamik Adapter Kaydı                  ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // Adapter factory
        var adapterRegistry = new Dictionary<string, IEmailService>();

        // Adapterleri kaydettir
        adapterRegistry["email"] = new SmtpEmailService();
        adapterRegistry["sms"] = new SmsToEmailAdapter(new TwilioSmsGateway());
        adapterRegistry["push"] = new PushToEmailAdapter(new FirebaseCloud());
        adapterRegistry["slack"] = new SlackToEmailAdapter(new SlackWebhook());

        Console.WriteLine("Kayıtlı kanallar:\n");
        foreach (var channel in adapterRegistry.Keys)
        {
            Console.WriteLine($"  ✓ {channel}");
        }

        Console.WriteLine("\n--- Dinamik Bildirim Gönderimi ---\n");

        // Bildirim gönder (dinamik)
        var notificationConfig = new[]
        {
            ("email", "admin@example.com", "Sistem Raporu", "Günlük rapor hazırlandı"),
            ("sms", "+905551234567", "Sistem Raporu", "Günlük rapor hazırlandı"),
            ("push", "device-admin-1", "Sistem Raporu", "Günlük rapor hazırlandı"),
            ("slack", "admin-channel", "Sistem Raporu", "Günlük rapor hazırlandı"),
        };

        foreach (var (channel, to, subject, body) in notificationConfig)
        {
            if (adapterRegistry.TryGetValue(channel, out var adapter))
            {
                Console.WriteLine($"▶ {channel.ToUpper()} kanalı kullanılıyor:");
                adapter.SendEmail(to, subject, body);
                Console.WriteLine();
            }
        }

        Console.WriteLine(new string('─', 60));
        Console.WriteLine("AVANTAJ: Yeni kanal eklemek için sadece registry'ye eklemek yeterli!");
        Console.WriteLine("         Code'da hiçbir değişiklik gerekmez.\n");
    }
}

/// <summary>
/// Main örnek çağırıcı
/// </summary>
public class AdapterExample
{
    public static void RunAll()
    {
        AdapterComparison.RunAll();
        ECommerceNotificationExample.Run();
        DynamicAdapterExample.Run();

        Console.WriteLine("\n" + new string('═', 70));
        Console.WriteLine("    ÖNEMLİ NOTLAR");
        Console.WriteLine(new string('═', 70));

        Console.WriteLine(@"
1. ADAPTER vs BRIDGE (Sık Karışan Kalıplar)
   • Adapter: Uyumsuz interface'leri uyumlu hale getir
   • Bridge: Implementation ve abstraction'ı ayırt

2. ADAPTER vs WRAPPER
   • Adapter: Interface conversion
   • Wrapper: Functionality enhancement
   • Decorator: Dinamik behavior ekleme

3. İYİ UYGULAMALAR
   • Adapter'lar küçük ve odaklı olmalı
   • Semantik çeviri yapılmalı
   • Çok karmaşık adaptasyonlar uyarı işareti

4. ANTI-PATTERN
   • Adapter zinciri (çok fazla katman)
   • Tamamen farklı semantik → Strategy tercih et
   • Adapter sadece lazy loading için → Dependency Injection tercih et

5. NE ZAMAN KULLANILIR
   • Third-party library integration
   • Legacy system modernization
   • Multi-channel support (Email, SMS, Push, Slack)
   • Hardware/Driver integration
   • Data format conversion
");

        Console.WriteLine(new string('═', 70) + "\n");
    }
}
