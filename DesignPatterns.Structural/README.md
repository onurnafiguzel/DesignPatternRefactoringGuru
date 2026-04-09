# Structural (Yapısal) Tasarım Kalıpları

Structural patterns, nesneler ve sınıflar arasındaki bileşim ilişkilerini ele alır. Var olan yapıları birleştirerek yeni, daha karmaşık yapılar oluşturmak için kullanılır.

## Bu Kategorideki Kalıplar

- **Adapter** — Uyumsuz interface'leri uyumlu hale getirme
- **Bridge** — Abstraction ve implementation ayrıştırması
- **Composite** — Ağaç yapılarında, parçalar ve bütün
- **Decorator** — Nesnelere dinamik olarak sorumluluk ekleme
- **Facade** — Karmaşık subsystem'leri basit interface'lerle kaplama
- **Proxy** — Başka nesnelere erişimi kontrol etme
- **Flyweight** — Benzer nesneleri paylaşarak bellek tasarrufu

## Adapter Kalıbı - Başlangıç Örneği

Adapter, **uyumsuz interface'leri birlikte çalışabilir hale getirir**.

**Senaryo**: Mevcut uygulamanız Email gönderiyor. Şimdi SMS de göndermek istiyorsunuz, ama SMS provider'ın API'si tamamen farklı. Varolan kodu değiştirmeden SMS'i Email gibi kullan.

Bakın: [Adapter/Problem.md](Adapter/Problem.md)
