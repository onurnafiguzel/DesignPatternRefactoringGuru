namespace DesignPatterns.Behavioral.ChainOfResponsibility.Correct;

/// <summary>
/// ✅ CHAIN OF RESPONSIBILITY: İsteği zincir boyunca geçir
///
/// Amaç:
/// - İsteği gönderen ile işleyen nesneyi birbirinden ayır
/// - Her halka işleyip işlemeyeceğine kendisi karar verir
/// - İşleyemezse zincirdeki bir sonrakine iletir
/// - Zincir runtime'da dinamik olarak kurulabilir
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Request (Zincir boyunca taşınan istek)
// ═══════════════════════════════════════════════════════════

public enum Priority { Low, Medium, High, Critical }

public class Ticket
{
    public string Description { get; }
    public Priority Priority { get; }
    public string Type { get; }

    public Ticket(string description, Priority priority, string type = "general")
    {
        Description = description;
        Priority = priority;
        Type = type;
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Abstract Handler (Zincir kurulum mantığı burada)
// ═══════════════════════════════════════════════════════════

public abstract class SupportHandler
{
    private SupportHandler? _next;

    // ✅ Fluent API ile zincir kur: bot.SetNext(level1).SetNext(expert)
    public SupportHandler SetNext(SupportHandler next)
    {
        _next = next;
        return next;
    }

    public virtual void Handle(Ticket ticket)
    {
        if (_next is not null)
            _next.Handle(ticket);
        else
            Console.WriteLine($"   ⚠️  Zincir sonu: \"{ticket.Description}\" işlenemedi!");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Concrete Handlers
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Seviye 0: Bot — sadece SSS tipindeki düşük öncelikli soruları çözer
/// </summary>
public class BotHandler : SupportHandler
{
    public override void Handle(Ticket ticket)
    {
        if (ticket.Type == "faq" && ticket.Priority == Priority.Low)
        {
            Console.WriteLine($"   🤖 Bot çözdü: \"{ticket.Description}\"");
            Console.WriteLine("      SSS veritabanından otomatik yanıt gönderildi.");
        }
        else
        {
            Console.WriteLine($"   🤖 Bot: Anlayamadım, 1. seviyeye iletiyorum...");
            base.Handle(ticket);
        }
    }
}

/// <summary>
/// Seviye 1: 1. Seviye Destek — Low ve Medium öncelikli soruları çözer
/// </summary>
public class Level1Handler : SupportHandler
{
    public override void Handle(Ticket ticket)
    {
        if (ticket.Priority <= Priority.Medium)
        {
            Console.WriteLine($"   👤 1. Seviye çözdü: \"{ticket.Description}\"");
            Console.WriteLine($"      Öncelik: {ticket.Priority} — standart süreçle çözüldü.");
        }
        else
        {
            Console.WriteLine($"   👤 1. Seviye: Yetkim yok, uzmana iletiyorum...");
            base.Handle(ticket);
        }
    }
}

/// <summary>
/// Seviye 2: Uzman — High öncelikli soruları çözer
/// </summary>
public class ExpertHandler : SupportHandler
{
    public override void Handle(Ticket ticket)
    {
        if (ticket.Priority == Priority.High)
        {
            Console.WriteLine($"   🔧 Uzman çözdü: \"{ticket.Description}\"");
            Console.WriteLine("      Teknik analiz yapıldı, çözüm uygulandı.");
        }
        else
        {
            Console.WriteLine($"   🔧 Uzman: Critical seviye, yöneticiye iletiyorum...");
            base.Handle(ticket);
        }
    }
}

/// <summary>
/// Seviye 3: Yönetici — Her şeyi işler (zincirin sonu)
/// </summary>
public class ManagerHandler : SupportHandler
{
    public override void Handle(Ticket ticket)
    {
        Console.WriteLine($"   👔 Yönetici işledi: \"{ticket.Description}\"");
        Console.WriteLine($"      Öncelik: {ticket.Priority} — özel ilgi gösterildi.");
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class ChainOfResponsibilityDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   ✅ CHAIN OF RESPONSIBILITY — Müşteri Destek Sistemi   ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // ✅ Zinciri kur (runtime'da değiştirilebilir)
        var bot     = new BotHandler();
        var level1  = new Level1Handler();
        var expert  = new ExpertHandler();
        var manager = new ManagerHandler();

        bot.SetNext(level1).SetNext(expert).SetNext(manager);

        // Ticketlar
        var tickets = new[]
        {
            new Ticket("Şifremi nasıl sıfırlarım?",   Priority.Low,      "faq"),
            new Ticket("Faturamda hata var",           Priority.Medium),
            new Ticket("Entegrasyon API'si çalışmıyor", Priority.High),
            new Ticket("Tüm sistem çöktü, veri kaybı!", Priority.Critical),
        };

        foreach (var ticket in tickets)
        {
            Console.WriteLine($"\n📩 Yeni Ticket [{ticket.Priority}]: \"{ticket.Description}\"");
            bot.Handle(ticket);
        }

        // ✅ Runtime'da zincir değiştirme (bot'u devre dışı bırak)
        Console.WriteLine("\n--- Bakım Modu: Bot devre dışı, zincir yeniden kuruldu ---\n");

        var level1Only = new Level1Handler();
        var expertOnly = new ExpertHandler();
        var managerOnly = new ManagerHandler();
        level1Only.SetNext(expertOnly).SetNext(managerOnly);

        var faqTicket = new Ticket("Şifremi nasıl sıfırlarım?", Priority.Low, "faq");
        Console.WriteLine($"📩 Yeni Ticket [{faqTicket.Priority}]: \"{faqTicket.Description}\"");
        level1Only.Handle(faqTicket);

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ Yeni seviye = yeni sınıf, mevcut kod değişmez");
        Console.WriteLine("✓ Zincir sırası runtime'da değiştirilebilir");
        Console.WriteLine("✓ Her halka sadece kendi kriterini bilir");
        Console.WriteLine("✓ Her handler izole test edilebilir\n");
    }
}
