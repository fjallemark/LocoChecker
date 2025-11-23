namespace Tellurian.Trains.Modules;

public struct LocoConfiguration
{
    public byte PrimaryAddress { get; set; } // CV1
    public byte DeaccellerationRate { get; set; } //CV4
    public byte ManufacturerVersion { get; set; } //CV7
    public byte ManufacturerId { get; set; } // CV8
    public byte DecoderNumber { get; set; } // CV16
    public byte ExtendedAddressMsb { get; set; } // CV17
    public byte ExtendedAddressLsb { get; set; } // CV18
    public byte ConsistAddress { get; set; } //CV19
    public byte ConsistAddressActiveF1ToF8 { get; set; } //CV21
    public byte ConsistAddressActiveFLAndF9ToF12 { get; set; } //CV22
    public CV29Flags ConfigurationVariable29 { get; set; } //CV29

    public readonly bool IsLongAddress => ConfigurationVariable29.IsLongAddress();
    public readonly bool IsAnalogOperationEnabled => ConfigurationVariable29.IsAnalogOperationEnabled();
    public readonly bool IsRailComEnabled => ConfigurationVariable29.IsRailComEnabled();
    public readonly bool Is28Or128SpeedSteps => ConfigurationVariable29.Is28Or128SpeedSteps();
    public int LocoAddress { readonly get { return this.LocoAddress(); } set { this.LocoAddress(value); } }
    internal int OriginalLocoAddress { get; set; }

    internal void UseShortAddress() => ConfigurationVariable29 = ConfigurationVariable29.WithShortAddress();
    internal void UseLongAddress() => ConfigurationVariable29 = ConfigurationVariable29.WithLongAddress();
    internal void DisableAnalogeOperation() => ConfigurationVariable29 = ConfigurationVariable29.WithDisabledAnalogOperation();
    internal void DisableRailCom() => ConfigurationVariable29 = ConfigurationVariable29.WithRailComDisabled();
}

public static class CvSetExtensions
{
    internal static int LocoAddress(this LocoConfiguration me) =>
        me.IsLongAddress ?
        ((me.ExtendedAddressMsb - 192) * 256) + me.ExtendedAddressLsb :
        me.PrimaryAddress;

    internal static void LocoAddress(this ref LocoConfiguration me, int address)
    {
        if (address < 1 || address == 3 || address > 9999) throw new ArgumentOutOfRangeException(nameof(address), $"Address {address} is invalid.");
        if (address < 128)
        {
            me.PrimaryAddress = (byte)address;
            me.UseShortAddress();
        }
        else
        {
            me.ExtendedAddressMsb = (byte)((address / 256) + 192);
            me.ExtendedAddressLsb = (byte)(address % 256);
            me.UseLongAddress();
        }
    }
}
