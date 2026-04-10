# Factory Method Kalıbı — Problem ve Çözüm

## 🎯 Senaryo

Bir lojistik uygulaması geliştiriyorsunuz. Başlangıçta yalnızca **kara taşımacılığı** vardı. Şimdi **deniz** ve **hava** taşımacılığı da eklendi. Her taşıma türünün farklı bir `Shipment` nesnesi var ve bu nesnelerin oluşturulma mantığı karmaşıklaşıyor.

---

## ❌ PROBLEM: Pattern Olmadan

```csharp
public class LogisticsService
{
    public void CreateShipment(string type, string destination, decimal weight)
    {
        // ❌ Oluşturma mantığı iş mantığına gömülü
        if (type == "road")
        {
            var truck = new Truck();
            truck.LoadCargo(weight);
            var shipment = new RoadShipment(truck, destination);
            shipment.Dispatch();
        }
        else if (type == "sea")
        {
            var ship = new CargoShip(capacity: 50000);
            ship.LoadCargo(weight);
            var shipment = new SeaShipment(ship, destination, port: "Istanbul");
            shipment.Dispatch();
        }
        else if (type == "air")
        {
            var plane = new FreightPlane(maxLoad: 5000);
            // ❌ Hava kargosunda ağırlık limiti kontrolü
            if (weight > 5000)
                throw new ArgumentException("Ağırlık limiti aşıldı");
            var shipment = new AirShipment(plane, destination, priority: "express");
            shipment.Dispatch();
        }
        // ❌ Yeni taşıma türü = buraya yeni if/else
    }
}
```

### Sorunlar:

1. **Oluşturma mantığı iş mantığına karışmış** → `CreateShipment` hem nesne oluşturuyor hem iş yapıyor
2. **Yeni taşıma türü eklemek** → `LogisticsService`'i değiştirmek gerekir
3. **Her tür için farklı kurulum detayları** → Karmaşıklık şişiyor
4. **Polimorfizm kullanılamıyor** → Ortak interface olsa da `new` doğrudan çağrılıyor
5. **Test zorluğu** → Gerçek `Truck`, `CargoShip` nesneleri oluşturuluyor, mock zor

---

## ✅ ÇÖZÜM: Factory Method

### Felsefe: "Hangi nesnenin oluşturulacağına alt sınıf karar versin"

```
LogisticsService (abstract)
├── CreateShipment()  ← iş mantığı burada, new yok
└── CreateTransport() ← abstract, alt sınıf implement eder

    ├── RoadLogistics  → CreateTransport() { return new Truck(); }
    ├── SeaLogistics   → CreateTransport() { return new CargoShip(); }
    └── AirLogistics   → CreateTransport() { return new FreightPlane(); }
```

### Kullanım:
```csharp
LogisticsService logistics = new RoadLogistics();
logistics.CreateShipment("Ankara", 1200);   // Truck kullanır

logistics = new SeaLogistics();
logistics.CreateShipment("Rotterdam", 45000); // CargoShip kullanır

// Yeni tür eklemek? LogisticsService'e dokunma, yeni sınıf yaz:
logistics = new DroneLogistics();
logistics.CreateShipment("Kadıköy", 2);       // Drone kullanır
```

---

## 📊 Karşılaştırma

| Özellik | OLMADAN | FACTORY METHOD |
|---------|---------|----------------|
| **Yeni tür ekleme** | Merkezi kod değişir ❌ | Yeni alt sınıf ✅ |
| **Oluşturma / iş ayrımı** | İç içe ❌ | Ayrı ✅ |
| **Polimorfizm** | Kullanılamıyor ❌ | Tam ✅ |
| **Test / Mock** | Zor ❌ | Kolay ✅ |
| **Open/Closed** | İhlal ❌ | Sağlı ✅ |

---

## 💡 Ne Zaman Kullanılır?

- 🚛 **Lojistik / taşımacılık** ← Örneğimiz
- 📄 **Döküman export** — PDF, Excel, Word factory
- 🔔 **Bildirim kanalları** — Email, SMS, Push factory
- 🗄️ **Veritabanı bağlantıları** — MySQL, PostgreSQL, SQLite factory
- 🎮 **Oyun nesneleri** — Düşman, silah, güç kaynağı üretimi
- 🧩 **UI bileşenleri** — Platform bazlı widget factory

Bakın: [Pattern.cs](Pattern.cs) — Tam implementasyon
