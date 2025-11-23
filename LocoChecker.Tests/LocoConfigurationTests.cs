namespace Tellurian.Trains.Modules.Tests;

[TestClass]
public class LocoConfigurationTests
{
    [TestMethod]
    public void GetLongAddressWorks()
    {
        var target = new LocoConfiguration
        {
            PrimaryAddress = 3,
            ExtendedAddressMsb = 197,
            ExtendedAddressLsb = 230,
            ConfigurationVariable29 = CV29Flags.ExtendedAddresInUse
        };
        Assert.AreEqual(1510, target.LocoAddress);
    }

    [TestMethod]
    public void GetShortAddressWorks()
    {
        var target = new LocoConfiguration
        {
            PrimaryAddress = 3,
            ExtendedAddressMsb = 197,
            ExtendedAddressLsb = 230,
            ConfigurationVariable29 = CV29Flags.None
        };
        Assert.AreEqual(3, target.LocoAddress);
    }

    [TestMethod]
    public void SetAddress3Throws()
    {
        var target = new LocoConfiguration();
        Assert.Throws<ArgumentOutOfRangeException>(() => target.LocoAddress = 3);
    }

    [TestMethod]
    public void SetShortAddressWorks()
    {
        var target = new LocoConfiguration { LocoAddress = 127 };
        Assert.AreEqual(127, target.LocoAddress);
        Assert.IsFalse(target.IsLongAddress);
    }

    [TestMethod]
    public void SetLongAddressWorks()
    {
        var target = new LocoConfiguration { LocoAddress = 128 };
        Assert.AreEqual(128, target.LocoAddress);
        Assert.IsTrue(target.IsLongAddress);
    }
}
