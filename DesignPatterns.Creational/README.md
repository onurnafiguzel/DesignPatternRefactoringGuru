# Creational (Yaratımsal) Tasarım Kalıpları

Creational patterns, nesnelerin oluşturulma mekanizmasıyla ilgilenir. Nesne oluşturmayı kontrol ederek, sistemin esnekliğini ve yeniden kullanılabilirliğini artırırlar.

## Bu Kategorideki Kalıplar

- **Singleton** — Bir sınıfın sadece bir örneğinin olması gerektiği durumlarda
- **Factory Method** — Nesnelerin oluşturulmasını alt sınıflara bırakma
- **Abstract Factory** — Birbiriyle ilişkili nesne ailelerinin oluşturulması
- **Builder** — Karmaşık nesneleri adım adım oluşturma
- **Prototype** — Mevcut nesneler kopyalayarak yeni nesneler oluşturma

## Singleton Kalıbı - Başlangıç Örneği

Singleton, **sadece bir örneğinin olması gerektiği nesneler** için kullanılır:
- Database Connection Pool
- Logger
- Configuration Manager
- Thread Pool
- Cache

**Senaryo**: Veritabanı bağlantı pool'u. Pool'un uygulama başında bir kez oluşturulup, tüm yerden aynı örneğin kullanılması gerekir.

Bakın: [Singleton/Problem.md](Singleton/Problem.md)
