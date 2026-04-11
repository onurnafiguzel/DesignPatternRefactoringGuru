namespace DesignPatterns.Creational.AbstractFactory.Correct;

/// <summary>
/// ✅ ABSTRACT FACTORY PATTERN: Uyumlu nesne ailesini üret
///
/// Amaç:
/// - Birbiriyle uyumlu nesneler ailesini üretmek için interface sağla
/// - Somut sınıflara bağımlı olmadan birden fazla ürünü oluştur
/// - Aile tutarlılığını garanti et (Win+Win, Mac+Mac)
/// - Yeni aile eklemek için mevcut kodu değiştirme
///
/// Kaynak: https://refactoring.guru/design-patterns/abstract-factory
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Abstract Products (Her bileşen için interface)
// ═══════════════════════════════════════════════════════════

public interface IButton
{
    void Render();
    void OnClick(Action handler);
}

public interface ICheckbox
{
    void Render();
    void Toggle();
    bool IsChecked { get; }
}

public interface ITextInput
{
    void Render();
    string GetValue();
    void SetPlaceholder(string text);
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Concrete Products — Windows Ailesi
// ═══════════════════════════════════════════════════════════

public class WindowsButton : IButton
{
    private Action? _clickHandler;
    public void Render()   => Console.WriteLine("   [Win] ▶ Button  [███████]");
    public void OnClick(Action handler) => _clickHandler = handler;
}

public class WindowsCheckbox : ICheckbox
{
    public bool IsChecked { get; private set; }
    public void Render()   => Console.WriteLine($"   [Win] ▶ Checkbox [{(IsChecked ? "✓" : " ")}]");
    public void Toggle()   => IsChecked = !IsChecked;
}

public class WindowsTextInput : ITextInput
{
    private string _placeholder = "";
    public void Render()               => Console.WriteLine($"   [Win] ▶ TextInput [_{_placeholder}_____]");
    public string GetValue()           => "win_input_value";
    public void SetPlaceholder(string text) => _placeholder = text;
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Concrete Products — macOS Ailesi
// ═══════════════════════════════════════════════════════════

public class MacButton : IButton
{
    private Action? _clickHandler;
    public void Render()   => Console.WriteLine("   [Mac] ◉ Button  (───────)");
    public void OnClick(Action handler) => _clickHandler = handler;
}

public class MacCheckbox : ICheckbox
{
    public bool IsChecked { get; private set; }
    public void Render()   => Console.WriteLine($"   [Mac] ◉ Checkbox ◯{(IsChecked ? "●" : " ")}");
    public void Toggle()   => IsChecked = !IsChecked;
}

public class MacTextInput : ITextInput
{
    private string _placeholder = "";
    public void Render()               => Console.WriteLine($"   [Mac] ◉ TextInput ╭{_placeholder}─────╮");
    public string GetValue()           => "mac_input_value";
    public void SetPlaceholder(string text) => _placeholder = text;
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Concrete Products — Linux Ailesi
// ═══════════════════════════════════════════════════════════

public class LinuxButton : IButton
{
    private Action? _clickHandler;
    public void Render()   => Console.WriteLine("   [GTK] ◆ Button  <───────>");
    public void OnClick(Action handler) => _clickHandler = handler;
}

public class LinuxCheckbox : ICheckbox
{
    public bool IsChecked { get; private set; }
    public void Render()   => Console.WriteLine($"   [GTK] ◆ Checkbox [{(IsChecked ? "x" : "_")}]");
    public void Toggle()   => IsChecked = !IsChecked;
}

public class LinuxTextInput : ITextInput
{
    private string _placeholder = "";
    public void Render()               => Console.WriteLine($"   [GTK] ◆ TextInput |{_placeholder}.......|");
    public string GetValue()           => "linux_input_value";
    public void SetPlaceholder(string text) => _placeholder = text;
}

// ═══════════════════════════════════════════════════════════
// STEP 5: Abstract Factory Interface
// ═══════════════════════════════════════════════════════════

public interface IUIFactory
{
    IButton     CreateButton();
    ICheckbox   CreateCheckbox();
    ITextInput  CreateTextInput();
}

// ═══════════════════════════════════════════════════════════
// STEP 6: Concrete Factories (Her aile kendi fabrikasında)
// ═══════════════════════════════════════════════════════════

public class WindowsUIFactory : IUIFactory
{
    public IButton    CreateButton()    => new WindowsButton();
    public ICheckbox  CreateCheckbox()  => new WindowsCheckbox();
    public ITextInput CreateTextInput() => new WindowsTextInput();
}

public class MacUIFactory : IUIFactory
{
    public IButton    CreateButton()    => new MacButton();
    public ICheckbox  CreateCheckbox()  => new MacCheckbox();
    public ITextInput CreateTextInput() => new MacTextInput();
}

public class LinuxUIFactory : IUIFactory
{
    public IButton    CreateButton()    => new LinuxButton();
    public ICheckbox  CreateCheckbox()  => new LinuxCheckbox();
    public ITextInput CreateTextInput() => new LinuxTextInput();
}

// ═══════════════════════════════════════════════════════════
// STEP 7: Client / Application (Factory'yi kullanır, ailelerden habersiz)
// ═══════════════════════════════════════════════════════════

public class LoginForm
{
    private readonly IButton    _submitButton;
    private readonly ICheckbox  _rememberMe;
    private readonly ITextInput _usernameInput;
    private readonly ITextInput _passwordInput;

    // ✅ Hangi platform? Bilmiyor. Factory ne verirse onu kullanıyor.
    public LoginForm(IUIFactory factory)
    {
        _submitButton  = factory.CreateButton();
        _rememberMe    = factory.CreateCheckbox();
        _usernameInput = factory.CreateTextInput();
        _passwordInput = factory.CreateTextInput();

        _usernameInput.SetPlaceholder("kullanıcı adı");
        _passwordInput.SetPlaceholder("şifre");
        _submitButton.OnClick(() => Console.WriteLine("   → Giriş yapılıyor..."));
    }

    public void Render()
    {
        Console.WriteLine("   ┌── Giriş Formu ──────────────┐");
        _usernameInput.Render();
        _passwordInput.Render();
        _rememberMe.Render();
        _submitButton.Render();
        Console.WriteLine("   └─────────────────────────────┘");
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class AbstractFactoryDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║    ✅ ABSTRACT FACTORY — Cross-Platform UI              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        // ✅ Factory seçimi tek bir noktada
        var factories = new (string Platform, IUIFactory Factory)[]
        {
            ("Windows", new WindowsUIFactory()),
            ("macOS",   new MacUIFactory()),
            ("Linux",   new LinuxUIFactory()),
        };

        foreach (var (platform, factory) in factories)
        {
            Console.WriteLine($"\n─── {platform} ───");
            // ✅ LoginForm hangi factory olduğunu bilmiyor
            // Aile tutarlılığı factory tarafından garanti edildi
            var form = new LoginForm(factory);
            form.Render();
        }

        // ✅ Aile tutarlılığı kanıtı
        Console.WriteLine("\n─── Aile Tutarlılığı ───");
        Console.WriteLine("Windows factory'den üretilen tüm bileşenler Win ailesi:");
        var winFactory = new WindowsUIFactory();
        winFactory.CreateButton().Render();
        winFactory.CreateCheckbox().Render();
        winFactory.CreateTextInput().Render();

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ LoginForm içinde if/else yok, platform bilmiyor");
        Console.WriteLine("✓ Aile tutarlılığı garanti — Mac factory Mac bileşen üretir");
        Console.WriteLine("✓ Yeni platform = 3 ürün sınıfı + 1 factory sınıfı");
        Console.WriteLine("✓ Mock factory ile LoginForm unit test edilebilir\n");
    }
}
