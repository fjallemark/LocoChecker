namespace Tellurian.Trains.Modules;

public interface ILocoAddressService
{
    LocoAddressBooking? TryGetAddress(int address);
    void BookAddress(LocoAddressBooking address);
}

public sealed class LocoAddressBooking
{
    public LocoAddressBooking(int address, string locoCompany, string locoClass, string locoNumber, string ownerName)
    {
        Address = address;
        Company = locoCompany;
        Class = locoClass;
        Number = locoNumber;
        Owner = ownerName;
    }
    public int Address { get; }
    public string Company { get; }
    public string Class { get; }
    public string Number { get; }
    public string Owner { get; }
    public string Loco => $"{Company} {Class} {Number}".Trim();
}
