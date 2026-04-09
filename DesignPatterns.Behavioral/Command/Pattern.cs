namespace DesignPatterns.Behavioral.Command.Correct;

/// <summary>
/// ✅ COMMAND PATTERN: İşlemleri nesneye kapsülle
///
/// Amaç:
/// - İsteği / işlemi bir nesne olarak temsil et
/// - Undo / Redo desteği ekle
/// - İşlem geçmişini tut
/// - Makro (işlem dizisi) desteği sağla
/// - Çağıranı (invoker) alıcıdan (receiver) ayır
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Command Interface
// ═══════════════════════════════════════════════════════════

public interface ICommand
{
    string Description { get; }
    void Execute();
    void Undo();
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Receiver (İşlemin yapıldığı nesne)
// ═══════════════════════════════════════════════════════════

public class Document
{
    public string Content { get; set; } = "";

    public void PrintContent()
        => Console.WriteLine($"   İçerik: \"{Content}\"");
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Concrete Commands
// ═══════════════════════════════════════════════════════════

public class InsertTextCommand : ICommand
{
    private readonly Document _document;
    private readonly string _text;
    private readonly int _position;

    public string Description => $"Metin ekle: \"{_text}\"";

    public InsertTextCommand(Document document, string text, int position = -1)
    {
        _document = document;
        _text = text;
        _position = position == -1 ? document.Content.Length : position;
    }

    public void Execute()
    {
        _document.Content = _document.Content.Insert(_position, _text);
        Console.WriteLine($"   ✓ Eklendi: \"{_text}\"");
    }

    public void Undo()
    {
        _document.Content = _document.Content.Remove(_position, _text.Length);
        Console.WriteLine($"   ↩ Geri alındı: \"{_text}\"");
    }
}

public class DeleteTextCommand : ICommand
{
    private readonly Document _document;
    private readonly int _count;
    private string _deletedText = "";

    public string Description => $"{_count} karakter sil";

    public DeleteTextCommand(Document document, int count)
    {
        _document = document;
        _count = count;
    }

    public void Execute()
    {
        _deletedText = _document.Content[^_count..];
        _document.Content = _document.Content[..^_count];
        Console.WriteLine($"   ✓ Silindi: \"{_deletedText}\"");
    }

    public void Undo()
    {
        _document.Content += _deletedText;
        Console.WriteLine($"   ↩ Geri alındı: \"{_deletedText}\"");
    }
}

public class BoldCommand : ICommand
{
    private readonly Document _document;
    private bool _wasAlreadyBold;

    public string Description => "Kalın yap";

    public BoldCommand(Document document)
        => _document = document;

    public void Execute()
    {
        _wasAlreadyBold = _document.Content.StartsWith("<b>");
        if (!_wasAlreadyBold)
        {
            _document.Content = $"<b>{_document.Content}</b>";
            Console.WriteLine("   ✓ Kalın formatı uygulandı");
        }
    }

    public void Undo()
    {
        if (!_wasAlreadyBold && _document.Content.StartsWith("<b>"))
        {
            _document.Content = _document.Content[3..^4];
            Console.WriteLine("   ↩ Kalın formatı kaldırıldı");
        }
    }
}

// Makro: Birden fazla command'ı tek bir command gibi çalıştır
public class MacroCommand : ICommand
{
    private readonly List<ICommand> _commands;
    public string Description => $"Makro ({_commands.Count} işlem)";

    public MacroCommand(List<ICommand> commands)
        => _commands = commands;

    public void Execute()
    {
        Console.WriteLine($"   ▶ Makro başlıyor ({_commands.Count} işlem)");
        foreach (var command in _commands)
            command.Execute();
    }

    public void Undo()
    {
        Console.WriteLine($"   ↩ Makro geri alınıyor");
        foreach (var command in Enumerable.Reverse(_commands))
            command.Undo();
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Invoker (Command'ları çalıştırır, history tutar)
// ═══════════════════════════════════════════════════════════

public class TextEditor
{
    private readonly Document _document = new();
    private readonly Stack<ICommand> _history = new();
    private readonly Stack<ICommand> _redoStack = new();

    public void Execute(ICommand command)
    {
        command.Execute();
        _history.Push(command);
        _redoStack.Clear();   // Yeni işlem sonrası redo temizlenir
        _document.PrintContent();
    }

    public void Undo()
    {
        if (_history.Count == 0) { Console.WriteLine("   Geri alınacak işlem yok"); return; }
        var command = _history.Pop();
        Console.WriteLine($"\n↩ Geri Al: {command.Description}");
        command.Undo();
        _redoStack.Push(command);
        _document.PrintContent();
    }

    public void Redo()
    {
        if (_redoStack.Count == 0) { Console.WriteLine("   Yeniden yapılacak işlem yok"); return; }
        var command = _redoStack.Pop();
        Console.WriteLine($"\n↪ Yeniden Yap: {command.Description}");
        command.Execute();
        _history.Push(command);
        _document.PrintContent();
    }

    public void PrintHistory()
    {
        Console.WriteLine("\n=== İŞLEM GEÇMİŞİ ===");
        var history = _history.ToArray();
        for (int i = history.Length - 1; i >= 0; i--)
            Console.WriteLine($"  {history.Length - i}. {history[i].Description}");
    }

    public Document Document => _document;
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class CommandPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         ✅ COMMAND PATTERN — Metin Editörü              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        var editor = new TextEditor();

        // İşlemler
        Console.WriteLine("--- İşlemler ---\n");
        editor.Execute(new InsertTextCommand(editor.Document, "Merhaba"));
        editor.Execute(new InsertTextCommand(editor.Document, " Dünya"));
        editor.Execute(new BoldCommand(editor.Document));

        // Undo
        Console.WriteLine("\n--- Undo ---\n");
        editor.Undo();  // Bold geri al
        editor.Undo();  // " Dünya" geri al

        // Redo
        Console.WriteLine("\n--- Redo ---\n");
        editor.Redo();  // " Dünya" geri getir

        // Makro
        Console.WriteLine("\n--- Makro ---\n");
        var macro = new MacroCommand(new List<ICommand>
        {
            new InsertTextCommand(editor.Document, "!"),
            new BoldCommand(editor.Document),
        });
        editor.Execute(macro);

        // Makro'yu geri al
        Console.WriteLine();
        editor.Undo();

        editor.PrintHistory();

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ Undo / Redo Stack ile tam geçmiş yönetimi");
        Console.WriteLine("✓ Makro: N işlemi tek command gibi çalıştır");
        Console.WriteLine("✓ Çağıran (editor) ile işlem (command) ayrı");
        Console.WriteLine("✓ Her command izole test edilebilir\n");
    }
}
