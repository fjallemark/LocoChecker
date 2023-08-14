using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8629 // Nullable value type may be null.

namespace Tellurian.Trains.Modules.Tests;

[TestClass]
public class LocoCheckerTests
{
    [TestMethod]
    public void MyTestMethod()
    {
        var target = Create(AllWrong);
        var (read, stored) = target.Read();
        Assert.IsInstanceOfType(read, typeof(LocoConfiguration));
        Assert.IsNull(stored);
        var messages = target.Analyse(read.Value);
        Assert.AreEqual(8, messages.Count());
    }

    private static LocoChecker Create(LocoConfiguration configuration) => new LocoChecker("Manufacturers.txt".Read, new TestDecoderService(configuration), new TestLocoAddressService(), new TestLocoConfigurationStore()  );

    public static LocoConfiguration AllWrong => new LocoConfiguration
    {
        ConfigurationVariable29 = CV29Flags.AnalogOperation | CV29Flags.RaicomActive,
        ConsistAddress = 1,
        DeaccellerationRate = 15,
        DecoderNumber = 1,
        LocoAddress = 1510,
        ManufacturerId = 157
    };
}

public class TestDecoderService : IDecoderService
{
    public TestDecoderService(LocoConfiguration instance)
    {
        Instance = instance;
    }
    private LocoConfiguration Instance;
    public LocoConfiguration? Read() => Instance;

    public void Write(LocoConfiguration configuration)
    {
        Instance = configuration;
    }
}

public class TestLocoAddressService : ILocoAddressService
{
    private readonly List<LocoAddressBooking> Bookings = new List<LocoAddressBooking>()
    {
        new LocoAddressBooking(1343, "GC", "Rc3", "1343", "Stefan Fjällemark")
    };

    public void BookAddress(LocoAddressBooking address) => Bookings.Add(address);
    public LocoAddressBooking? TryGetAddress(int address) => Bookings.SingleOrDefault(b => b.Address == address);
}

public class TestLocoConfigurationStore : ILocoConfigurationStore
{
    private readonly Dictionary<int, LocoConfiguration> Stored = new Dictionary<int, LocoConfiguration>();
    public void Save(LocoConfiguration configuration) => Stored[configuration.LocoAddress] = configuration;
    public LocoConfiguration? TryLoad(int address) => Stored.ContainsKey(address) ? Stored[address] : (LocoConfiguration?)null;
}
