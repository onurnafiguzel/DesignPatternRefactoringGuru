# Command Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

Bir metin editörü geliştiriyorsunuz. Kullanıcılar yazı yazıyor, siliyor, biçimlendiriyor. Editörün şu özellikleri desteklemesi gerekiyor:

- **Undo / Redo** — Son işlemi geri al / yeniden yap
- **Makro Kaydetme** — Birden fazla işlemi kaydet, tekrar çalıştır
- **İşlem Geçmişi** — Hangi işlemler yapıldı?

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class TextEditor
{
    private string _content = "";

    public void Bold()
    {
        _content = $"<b>{_content}</b>";
        Console.WriteLine("Kalın yapıldı");
        // ❌ Geri alma nasıl? Önceki state kayboldu!
    }

    public void InsertText(string text)
    {
        _content += text;
        // ❌ Önceki içeriği saklamadık, undo yok
    }

    public void Delete(int count)
    {
        _content = _content[..^count];
        // ❌ Ne silindi? Geri alınamaz!
    }
}
```

### Sorunlar:

1. **Undo imkansız** → Önceki state'i hiçbir işlem saklamıyor
2. **İşlem geçmişi yok** → Kim ne yaptı bilinmiyor
3. **Makro desteği yok** → İşlemler nesneler değil, çağrılar
4. **Tight coupling** → Çağıran (buton, kısayol, menü) doğrudan metod çağırır
5. **Kuyruk / zamanlama yok** → İşlemleri sıraya alıp sonra çalıştıramazsın

---

## ✅ ÇÖZÜM: Command Pattern

### Felsefe: "İsteği bir nesneye kapsülle"

Her işlem için **Execute** ve **Undo** methodlarını taşıyan bir nesne oluştur. Editör bu nesneleri bilmez, sadece çalıştırır ve history'ye ekler.

```
Invoker (Editor)
└── history: ICommand[]
    ├── InsertTextCommand  { Execute(), Undo() }
    ├── BoldCommand        { Execute(), Undo() }
    └── DeleteCommand      { Execute(), Undo() }
```

### Kullanım:
```csharp
var editor = new TextEditor();

editor.Execute(new InsertTextCommand(editor, "Merhaba"));
editor.Execute(new InsertTextCommand(editor, " Dünya"));
editor.Execute(new BoldCommand(editor));

editor.Undo();  // Bold geri alındı
editor.Undo();  // " Dünya" silindi
editor.Redo();  // " Dünya" geri geldi
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | COMMAND |
|---------|---------|---------|
| **Undo / Redo** | Yok ❌ | Var ✅ |
| **İşlem Geçmişi** | Yok ❌ | Tam log ✅ |
| **Makro** | Zor ❌ | Kolay ✅ |
| **Kuyruk / Zamanlama** | Yok ❌ | Var ✅ |
| **Test** | Zor ❌ | İzole ✅ |

---

## 💡 Ne Zaman Kullanılır?

- ✏️ **Metin / Grafik editörler** ← Örneğimiz (Undo/Redo)
- 🛒 **Sipariş sistemleri** — Siparişi kuyruğa al, sonra işle
- 🔄 **Transaction yönetimi** — Commit / Rollback
- 🎮 **Oyun sistemleri** — Hamle geçmişi, replay
- 📡 **Uzaktan komut çalıştırma** — RPC, job queue

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
