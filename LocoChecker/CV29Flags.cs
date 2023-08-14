namespace Tellurian.Trains.Modules;

[Flags]
public enum CV29Flags : byte
{
    None = 0x00,
    LocomotiveDirectionReverse = 0b0000001,
    SpeedSteps28or128 = 0b0000010,
    AnalogOperation = 0b0000100,
    RaicomActive = 0b0001000,
    DetailedSpeedCurve = 0b0010000,
    ExtendedAddresInUse = 0b0100000,
    NotPermitted = AnalogOperation | RaicomActive
}

internal static class CV29FlagsExtensions
{
    public static bool IsLongAddress(this CV29Flags flags) =>
        (flags & CV29Flags.ExtendedAddresInUse) > 0;
    public static bool IsAnalogOperationEnabled(this CV29Flags flags) =>
         (flags & CV29Flags.AnalogOperation) > 0;
    public static bool IsRailComEnabled(this CV29Flags flags) =>
        (flags & CV29Flags.RaicomActive) > 0;
    public static bool Is28Or128SpeedSteps(this CV29Flags flags) =>
        (flags & CV29Flags.SpeedSteps28or128) > 0;

    public static CV29Flags WithShortAddress(this CV29Flags flags) =>
        flags &= ~CV29Flags.ExtendedAddresInUse;
    public static CV29Flags WithLongAddress(this CV29Flags flags) =>
        flags |= CV29Flags.ExtendedAddresInUse;
    public static CV29Flags WithDisabledAnalogOperation(this CV29Flags flags) =>
         flags &= ~CV29Flags.AnalogOperation;
    public static CV29Flags WithRailComDisabled(this CV29Flags flags) =>
        flags &= ~CV29Flags.RaicomActive;
}
