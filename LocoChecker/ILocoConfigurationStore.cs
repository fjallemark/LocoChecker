namespace Tellurian.Trains.Modules
{
    public interface ILocoConfigurationStore
    {
        LocoConfiguration? TryLoad(int address);
        void Save(LocoConfiguration configuration);
    }
    public interface IDecoderService
    {
        LocoConfiguration? Read();
        void Write(LocoConfiguration configuration);
    }
}
