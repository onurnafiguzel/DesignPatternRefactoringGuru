# Decorator Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

> Bu örnek [refactoring.guru/design-patterns/decorator](https://refactoring.guru/design-patterns/decorator) adresindeki senaryodan esinlenmiştir.

Bir veri transferi kütüphanesi geliştiriyorsunuz. Temel `FileDataSource` sınıfınız var: dosyaya yazar/okur. Zamanla şu kombinasyonlar gerekti:

- Dosyaya yaz
- Şifreli dosyaya yaz
- Sıkıştırılmış dosyaya yaz
- **Şifreli + sıkıştırılmış** dosyaya yaz
- Şifreli + sıkıştırılmış + önbellekli dosyaya yaz

---

## ❌ PROBLEM: Pattern Olmadan

### Problem 1 — Alt Sınıf Patlaması

```csharp
public class FileDataSource { }
public class EncryptedFileDataSource : FileDataSource { }
public class CompressedFileDataSource : FileDataSource { }
public class CachedFileDataSource : FileDataSource { }

// ❌ Kombinasyonlar için alt sınıflar şişiyor
public class EncryptedCompressedFileDataSource : FileDataSource { }
public class EncryptedCachedFileDataSource : FileDataSource { }
public class CompressedCachedFileDataSource : FileDataSource { }
public class EncryptedCompressedCachedFileDataSource : FileDataSource { }
// 3 özellik = 2³ = 8 sınıf, 4 özellik = 16, 5 özellik = 32...
```

### Problem 2 — Runtime'da Davranış Eklenemiyor

```csharp
// ❌ Kullanıcı config'e göre şifreleme açık/kapalı olsun istiyoruz
// Ama kalıtımda bu mümkün değil — compile-time'da sabit
var source = userConfig.Encrypt
    ? new EncryptedFileDataSource()    // ❌ Ayrı sınıf
    : new FileDataSource();
```

### Sorunlar:

1. **Kombinasyon patlaması** → N özellik = 2ᴺ alt sınıf
2. **Runtime esnekliği yok** → Özellikler compile-time'da sabit
3. **Kod tekrarı** → Şifreleme mantığı birden fazla alt sınıfta kopyalanıyor
4. **Tek Sorumluluk ihlali** → Alt sınıf hem I/O hem şifreleme hem sıkıştırma yapıyor

---

## ✅ ÇÖZÜM: Decorator Pattern

### Felsefe: "Nesneyi saran bir sarmalayıcı ile davranış ekle"

```
IDataSource
├── FileDataSource          ← Temel bileşen
└── DataSourceDecorator     ← Soyut sarmalayıcı
    ├── EncryptionDecorator  → şifrele, sonra sar
    ├── CompressionDecorator → sıkıştır, sonra sar
    └── CacheDecorator       → önbellekle, sonra sar
```

### Kullanım:
```csharp
IDataSource source = new FileDataSource("data.txt");

// Runtime'da katmanları sar — istediğin kombinasyon
source = new EncryptionDecorator(source);
source = new CompressionDecorator(source);

source.WriteData("içerik");
// 1. CompressionDecorator.Write → sıkıştır
// 2. EncryptionDecorator.Write  → şifrele
// 3. FileDataSource.Write       → diske yaz
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | DECORATOR |
|---------|---------|-----------|
| **Kombinasyon sayısı** | 2ᴺ sınıf ❌ | N sınıf ✅ |
| **Runtime esnekliği** | Yok ❌ | Tam ✅ |
| **Kod tekrarı** | Yüksek ❌ | Yok ✅ |
| **Sıra kontrolü** | İmkansız ❌ | Sarma sırası ✅ |
| **Single Responsibility** | İhlal ❌ | Her decorator tek iş ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 💾 **I/O stream'leri** ← Örneğimiz (Java'nın InputStream mimarisi aynen bu)
- 🔐 **Güvenlik katmanları** — Auth, rate-limit, logging middleware
- 🔔 **Bildirim sistemleri** — Email + SMS + Slack zinciri
- 🖌️ **UI bileşenleri** — Scroll, border, shadow eklemek
- 📊 **Logging / Caching** — Metodlara cross-cutting concern eklemek
- 🌐 **HTTP middleware** — ASP.NET Core pipeline tam olarak Decorator

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
