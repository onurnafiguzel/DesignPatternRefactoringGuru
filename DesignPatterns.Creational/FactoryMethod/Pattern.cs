namespace DesignPatterns.Creational.FactoryMethod.Correct;

/// <summary>
/// ✅ FACTORY METHOD PATTERN: Nesne oluşturmayı alt sınıflara bırak
///
/// Amaç:
/// - Nesne oluşturma mantığını iş mantığından ayır
/// - Hangi nesnenin oluşturulacağına alt sınıf karar versin
/// - İstemci kod somut sınıfları bilmeden çalışsın
/// - Yeni tür eklemek için mevcut kodu değiştirme
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Product Interface (Factory'den çıkan nesne)
// ═══════════════════════════════════════════════════════════

public interface ITransport
{
    string Name { get; }
    decimal MaxLoad { get; }   // kg
    void LoadCargo(decimal weight);
    void Dispatch(string destination);
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Concrete Products
// ═══════════════════════════════════════════════════════════

public class Truck : ITransport
{
    public string Name => "Kamyon";
    public decimal MaxLoad => 20_000;

    public void LoadCargo(decimal weight)
        => Console.WriteLine($"   🚛 Kamyona yüklendi: {weight} kg");

    public void Dispatch(string destination)
        => Console.WriteLine($"   🚛 Kamyon yola çıktı → {destination} (Kara yolu)");
}

public class CargoShip : ITransport
{
    public string Name => "Kargo Gemisi";
    public decimal MaxLoad => 500_000;

    public void LoadCargo(decimal weight)
        => Console.WriteLine($"   🚢 Gemiye yüklendi: {weight} kg");

    public void Dispatch(string destination)
        => Console.WriteLine($"   🚢 Gemi limandan ayrıldı → {destination} (Deniz yolu)");
}

public class FreightPlane : ITransport
{
    public string Name => "Kargo Uçağı";
    public decimal MaxLoad => 5_000;

    public void LoadCargo(decimal weight)
        => Console.WriteLine($"   ✈️  Uçağa yüklendi: {weight} kg");

    public void Dispatch(string destination)
        => Console.WriteLine($"   ✈️  Uçak havalandı → {destination} (Hava yolu)");
}

public class Drone : ITransport
{
    public string Name => "Drone";
    public decimal MaxLoad => 5;

    public void LoadCargo(decimal weight)
        => Console.WriteLine($"   🚁 Drone'a yüklendi: {weight} kg");

    public void Dispatch(string destination)
        => Console.WriteLine($"   🚁 Drone kalktı → {destination} (Son mil teslimat)");
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Creator (Factory Method burada tanımlanır)
// ═══════════════════════════════════════════════════════════

public abstract class LogisticsService
{
    // ✅ FACTORY METHOD: Alt sınıf hangi transport'u yaratacağına karar verir
    protected abstract ITransport CreateTransport();

    // ✅ İş mantığı burada — new çağrısı yok, somut sınıf bilinmiyor
    public void CreateShipment(string destination, decimal weight)
    {
        var transport = CreateTransport();   // Factory method çağrısı

        Console.WriteLine($"\n📦 Sevkiyat: {destination} | {weight} kg | {transport.Name}");

        if (weight > transport.MaxLoad)
        {
            Console.WriteLine($"   ❌ Yük limiti aşıldı! Max: {transport.MaxLoad} kg");
            return;
        }

        transport.LoadCargo(weight);
        transport.Dispatch(destination);
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Concrete Creators (Her biri farklı transport üretir)
// ═══════════════════════════════════════════════════════════

public class RoadLogistics : LogisticsService
{
    protected override ITransport CreateTransport() => new Truck();
}

public class SeaLogistics : LogisticsService
{
    protected override ITransport CreateTransport() => new CargoShip();
}

public class AirLogistics : LogisticsService
{
    protected override ITransport CreateTransport() => new FreightPlane();
}

// ✅ Yeni tür eklemek: LogisticsService'e dokunmadan yeni sınıf
public class DroneLogistics : LogisticsService
{
    protected override ITransport CreateTransport() => new Drone();
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class FactoryMethodDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║       ✅ FACTORY METHOD — Lojistik Sistemi              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

        // ✅ İstemci kod LogisticsService tipiyle çalışır
        // Somut sınıflar (Truck, CargoShip) görünmez
        var services = new LogisticsService[]
        {
            new RoadLogistics(),
            new SeaLogistics(),
            new AirLogistics(),
            new DroneLogistics(),
        };

        var orders = new (string Destination, decimal Weight)[]
        {
            ("Ankara",     1_200),
            ("Rotterdam",  48_000),
            ("New York",   3_500),
            ("Kadıköy",    3),
        };

        for (int i = 0; i < services.Length; i++)
            services[i].CreateShipment(orders[i].Destination, orders[i].Weight);

        // Limit aşımı senaryosu
        Console.WriteLine("\n─── Yük Limiti Aşımı Senaryosu ───");
        new AirLogistics().CreateShipment("Tokyo", 9_000);  // Max 5000 kg

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ LogisticsService içinde new Truck/Ship/Plane yok");
        Console.WriteLine("✓ Yeni taşıma türü = sadece 2 yeni sınıf");
        Console.WriteLine("✓ İstemci kod soyut tip üzerinden çalışıyor");
        Console.WriteLine("✓ Transport mock'lanarak servis test edilebilir\n");
    }
}
