namespace DesignPatterns.Creational.Builder.Correct;

/// <summary>
/// ✅ BUILDER PATTERN: Karmaşık nesneyi adım adım inşa et
///
/// Amaç:
/// - Karmaşık nesne oluşturmayı construction kodundan ayır
/// - Aynı inşa süreci farklı ürünler üretebilsin
/// - İsteğe bağlı bileşenler temiz bir şekilde yönetilsin
/// - Director ile inşa adımlarının sırası kontrol edilsin
///
/// Kaynak: https://refactoring.guru/design-patterns/builder
/// </summary>

// ═══════════════════════════════════════════════════════════
// STEP 1: Product (İnşa edilen karmaşık nesne)
// ═══════════════════════════════════════════════════════════

public class House
{
    private List<string> _parts = new();

    public string Type { get; set; } = "";

    public void AddPart(string part) => _parts.Add(part);

    public void Show()
    {
        Console.WriteLine($"\n🏠 {Type}");
        Console.WriteLine("   " + new string('─', 40));
        foreach (var part in _parts)
            Console.WriteLine($"   ✓ {part}");
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 2: Builder Interface (İnşa adımları sözleşmesi)
// ═══════════════════════════════════════════════════════════

public interface IHouseBuilder
{
    IHouseBuilder BuildFoundation();
    IHouseBuilder BuildWalls();
    IHouseBuilder BuildRoof();
    IHouseBuilder BuildGarage();
    IHouseBuilder BuildSwimmingPool();
    IHouseBuilder BuildGarden();
    IHouseBuilder BuildSolarPanels();
    House GetResult();
}

// ═══════════════════════════════════════════════════════════
// STEP 3: Concrete Builders
// ═══════════════════════════════════════════════════════════

public class SimpleHouseBuilder : IHouseBuilder
{
    private House _house = new();

    // ✅ Her Build metodu sonunda this döner → Fluent API
    public IHouseBuilder BuildFoundation()
    {
        _house.AddPart("Temel: Ahşap zemin");
        return this;
    }

    public IHouseBuilder BuildWalls()
    {
        _house.AddPart("Duvarlar: Ahşap panel");
        return this;
    }

    public IHouseBuilder BuildRoof()
    {
        _house.AddPart("Çatı: Sac kaplama");
        return this;
    }

    // Basit evde garaj, havuz, bahçe yok — adımlar boş geçilebilir
    public IHouseBuilder BuildGarage()       { return this; }
    public IHouseBuilder BuildSwimmingPool() { return this; }
    public IHouseBuilder BuildGarden()       { return this; }
    public IHouseBuilder BuildSolarPanels()  { return this; }

    public House GetResult()
    {
        _house.Type = "Basit Kulübe";
        var result = _house;
        _house = new House();   // Builder'ı sıfırla
        return result;
    }
}

public class StandardHouseBuilder : IHouseBuilder
{
    private House _house = new();

    public IHouseBuilder BuildFoundation()
    {
        _house.AddPart("Temel: Betonarme");
        return this;
    }

    public IHouseBuilder BuildWalls()
    {
        _house.AddPart("Duvarlar: Tuğla + sıva");
        return this;
    }

    public IHouseBuilder BuildRoof()
    {
        _house.AddPart("Çatı: Kiremit");
        return this;
    }

    public IHouseBuilder BuildGarage()
    {
        _house.AddPart("Garaj: 1 araçlık");
        return this;
    }

    public IHouseBuilder BuildGarden()
    {
        _house.AddPart("Bahçe: Ön bahçe düzenlemesi");
        return this;
    }

    public IHouseBuilder BuildSwimmingPool() { return this; }
    public IHouseBuilder BuildSolarPanels()  { return this; }

    public House GetResult()
    {
        _house.Type = "Standart Ev";
        var result = _house;
        _house = new House();
        return result;
    }
}

public class LuxuryHouseBuilder : IHouseBuilder
{
    private House _house = new();

    public IHouseBuilder BuildFoundation()
    {
        _house.AddPart("Temel: Güçlendirilmiş betonarme + yalıtım");
        return this;
    }

    public IHouseBuilder BuildWalls()
    {
        _house.AddPart("Duvarlar: Çift cidarlı ısı yalıtımlı tuğla");
        return this;
    }

    public IHouseBuilder BuildRoof()
    {
        _house.AddPart("Çatı: Çift katmanlı kiremit + termal yalıtım");
        return this;
    }

    public IHouseBuilder BuildGarage()
    {
        _house.AddPart("Garaj: 3 araçlık + elektrikli kapı");
        return this;
    }

    public IHouseBuilder BuildSwimmingPool()
    {
        _house.AddPart("Yüzme Havuzu: Isıtmalı, 10×5m");
        return this;
    }

    public IHouseBuilder BuildGarden()
    {
        _house.AddPart("Bahçe: Peyzajlı, sulama sistemi, aydınlatma");
        return this;
    }

    public IHouseBuilder BuildSolarPanels()
    {
        _house.AddPart("Güneş Panelleri: 20 panel, 8kW");
        return this;
    }

    public House GetResult()
    {
        _house.Type = "Lüks Villa";
        var result = _house;
        _house = new House();
        return result;
    }
}

// ═══════════════════════════════════════════════════════════
// STEP 4: Director (İnşa adımlarının sırasını yönetir)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Director, builder'ı nasıl kullanacağını bilir.
/// İstemci koddan inşa detaylarını gizler.
/// Director olmadan builder doğrudan da kullanılabilir.
/// </summary>
public class HouseDirector
{
    // Basit kulübe: sadece zorunlu adımlar
    public void BuildCabin(IHouseBuilder builder)
    {
        builder
            .BuildFoundation()
            .BuildWalls()
            .BuildRoof();
    }

    // Standart ev: garaj ve bahçe dahil
    public void BuildFamilyHome(IHouseBuilder builder)
    {
        builder
            .BuildFoundation()
            .BuildWalls()
            .BuildRoof()
            .BuildGarage()
            .BuildGarden();
    }

    // Lüks villa: her şey dahil
    public void BuildLuxuryVilla(IHouseBuilder builder)
    {
        builder
            .BuildFoundation()
            .BuildWalls()
            .BuildRoof()
            .BuildGarage()
            .BuildSwimmingPool()
            .BuildGarden()
            .BuildSolarPanels();
    }
}

// ═══════════════════════════════════════════════════════════
// DEMO
// ═══════════════════════════════════════════════════════════

public class BuilderPatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         ✅ BUILDER PATTERN — Ev İnşaatı                 ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

        var director = new HouseDirector();

        // Director ile önceden tanımlanmış yapılar
        Console.WriteLine("\n─── Director ile İnşaat ───");

        var simpleBuilder = new SimpleHouseBuilder();
        director.BuildCabin(simpleBuilder);
        simpleBuilder.GetResult().Show();

        var standardBuilder = new StandardHouseBuilder();
        director.BuildFamilyHome(standardBuilder);
        standardBuilder.GetResult().Show();

        var luxuryBuilder = new LuxuryHouseBuilder();
        director.BuildLuxuryVilla(luxuryBuilder);
        luxuryBuilder.GetResult().Show();

        // ✅ Director olmadan: Müşteri kendi kombinasyonunu seçer
        Console.WriteLine("\n─── Director Olmadan: Özel Sipariş ───");
        var customBuilder = new StandardHouseBuilder();
        customBuilder
            .BuildFoundation()
            .BuildWalls()
            .BuildRoof()
            .BuildSolarPanels();   // Standart'a güneş paneli ekle
        customBuilder.GetResult().Show();

        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("AVANTAJLAR:");
        Console.WriteLine("✓ new House(2, 5, true, false, ...) yok — okunabilir adımlar");
        Console.WriteLine("✓ Director aynı adımlarla farklı ürünler üretiyor");
        Console.WriteLine("✓ GetResult() nesneyi tutarlı halde teslim ediyor");
        Console.WriteLine("✓ Fluent API ile özel kombinasyon mümkün\n");
    }
}
