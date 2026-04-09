# Behavioral (Davranışsal) Tasarım Kalıpları

Behavioral patterns, nesneler arasındaki iletişim ve sorumluluk dağılımıyla ilgilenir. Nesnelerin nasıl etkileşim kurduğunu ve davranışlarını nasıl tanımladığını ele alır.

## Bu Kategorideki Kalıplar

- **Observer** — Nesneler arası loose coupling ile event notification
- **Strategy** — Runtime'da algoritma değiştirme
- **State** — Durum değiştikçe davranışı değiştirme
- **Command** — İşlemleri nesneler olarak kapsülleme
- **Iterator** — Collection öğelerine sıra ile erişim
- **Mediator** — Nesneler arası karmaşık iletişimi merkezi bir nesne ile yönetme
- **Memento** — Nesnenin previous state'ini geri yükleyebilme
- **Template Method** — Algoritmanın iskeletini tanımlama, alt sınıflara detayları bırakma
- **Visitor** — Elemanlar üzerinde işlem tanımlamadan yeni işlemler ekleme
- **Chain of Responsibility** — İsteği bir zincir boyunca geçirme

## Observer Kalıbı - Başlangıç Örneği

Observer, **loosely-coupled event-driven sistemler** için kullanılır.

**Senaryo**: Hisse senedi fiyatı değiştiğinde, portföy güncellemesi, uyarı sistemi ve rapor oluşturucu aynı anda harekete geçmesi gerekiyor. Fakat Stock sınıfı bu hizmetlerin detaylarını bilmemeli.

Bakın: [Observer/Problem.md](Observer/Problem.md)
