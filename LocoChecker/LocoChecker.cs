using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

[assembly: NeutralResourcesLanguage("en-UK")]

namespace Tellurian.Trains.Modules
{
    public class LocoChecker
    {
        public LocoChecker(Func<IEnumerable<Manufacturer>> getManufacturers, IDecoderService decoderService, ILocoAddressService locoAddressService, ILocoConfigurationStore configurationStore)
        {
            if (getManufacturers is null) throw new ArgumentNullException(nameof(getManufacturers));
            Manufacturers = getManufacturers();
            DecoderService = decoderService ?? throw new ArgumentNullException(nameof(decoderService));
            LocoAddressService = locoAddressService ?? throw new ArgumentNullException(nameof(locoAddressService));
            ConfigurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));
        }

        private readonly IEnumerable<Manufacturer> Manufacturers;
        private readonly IDecoderService DecoderService;
        private readonly ILocoAddressService LocoAddressService;
        private readonly ILocoConfigurationStore ConfigurationStore;

        /// <summary>
        /// Reads configuration from loco decoder and loads stored configuration if it extists.
        /// If a stored configuration exists, the user should also have an option to <see cref="RestoreSaved(int)">restore</see> the original settings.
        /// </summary>
        /// <returns></returns>
        public (LocoConfiguration? read, LocoConfiguration? stored) Read()
        {
            if (DecoderService.Read() is LocoConfiguration read)
            {
                if (ConfigurationStore.TryLoad(read.LocoAddress) is LocoConfiguration stored)
                    return (read, stored);
                else
                    return (read, null);
            }
            return (null, null);
        }

        public IEnumerable<Message> Analyse(LocoConfiguration configuration) =>
            LocoAddressService.TryGetAddress(configuration.LocoAddress) is LocoAddressBooking booking ?
            configuration.Analyse(Manufacturers, booking) :
            configuration.Analyse(Manufacturers);

        public void SaveCorrected(LocoConfiguration configuration, bool resetConsistAddress, bool resetDeacceleration, int? newLocoAddress) =>
            SaveAndWrite(configuration.Corrected(resetConsistAddress, resetDeacceleration, newLocoAddress));

        /// <summary>
        /// Restore saved <see cref="LocoConfiguration"/> to loco.
        /// </summary>
        /// <param name="address">The address of the loco when saved as corrected.</param>
        public void RestoreSaved(int address)
        {
            if (ConfigurationStore.TryLoad(address) is LocoConfiguration stored) DecoderService.Write(stored.Restored());
        }

        private void SaveAndWrite(LocoConfiguration configuration)
        {
            ConfigurationStore.Save(configuration);
            DecoderService.Write(configuration);
        }
    }

    internal static class LocoCheckerExtensions
    {
        public static IEnumerable<Message> Analyse(this LocoConfiguration configuration, IEnumerable<Manufacturer> manufacturers, LocoAddressBooking? existingBooking = null)
        {
            var messages = new List<Message>(30)
            {
                new Message(MessageSeverity.Information, Strings.DecoderManufacturer, manufacturers.SingleOrDefault(m => m.Number == configuration.ManufacturerId)?.Name ?? Strings.Unknown),
                new Message(MessageSeverity.Information, configuration.IsLongAddress ? Strings.LongAddress : Strings.ShortAddress, configuration.LocoAddress),
                new Message(MessageSeverity.Information, Strings.SpeedSteps, configuration.Is28Or128SpeedSteps ? "28/128" : "14")
            };
            if (existingBooking != null) messages.Add(new Message(MessageSeverity.Warning, string.Format(CultureInfo.CurrentCulture, Strings.ExistingLocoAddressWarning, existingBooking.Address, existingBooking.Owner, existingBooking.Loco)));
            if (configuration.ConsistAddress > 0) messages.Add(new Message(MessageSeverity.Suggestion, Strings.CV19Warning, configuration.ConsistAddress));
            if (configuration.DeaccellerationRate > 5) messages.Add(new Message(MessageSeverity.Suggestion, Strings.DeaccelerationWarning, configuration.DeaccellerationRate));
            if (configuration.IsAnalogOperationEnabled) messages.Add(new Message(MessageSeverity.Warning, Strings.AnalogOperationWillBeDIsabled));
            if (configuration.IsRailComEnabled) messages.Add(new Message(MessageSeverity.Warning, Strings.RailComWillBeDisabled));
            if (configuration.DecoderNumber > 0) messages.Add(new Message(MessageSeverity.Information, Strings.DecoderNumberIsNonZero, configuration.DecoderNumber));
            return messages;
        }

        public static LocoConfiguration Corrected(this LocoConfiguration original, bool resetConsistAddress, bool resetDeacceleration, int? newLocoAddress)
        {
            original.DisableAnalogeOperation();
            original.DisableRailCom();
            if (resetConsistAddress) original.ConsistAddress = 0;
            if (resetDeacceleration) original.DeaccellerationRate = 1;
            original.OriginalLocoAddress = original.LocoAddress;
            if (newLocoAddress.HasValue) original.LocoAddress = newLocoAddress.Value;
            return original;
        }

        public static LocoConfiguration Restored(this LocoConfiguration stored)
        {
            if (stored.LocoAddress != stored.OriginalLocoAddress) stored.LocoAddress = stored.OriginalLocoAddress;
            return stored;
        }
    }

    public class Message
    {
        public Message(MessageSeverity severity, string text)
        {
            Severity = severity;
            Text = text;
        }
        public Message(MessageSeverity severity, string format, object arg0)
        {
            Severity = severity;
            Text = string.Format(CultureInfo.CurrentCulture, format, arg0);
        }
        public MessageSeverity Severity { get; }
        public string Text { get; }

        public override string ToString() => $"{Severity.ToLocalizedString()}: {Text}";
    }

    public enum MessageSeverity
    {
        Information,
        Suggestion,
        Warning
    }
    public static class MessageSeverityExtensions
    {
        public static string ToLocalizedString(this MessageSeverity me) =>
            me switch
            {
                MessageSeverity.Information => Strings.Information,
                MessageSeverity.Suggestion => Strings.Suggestion,
                MessageSeverity.Warning => Strings.Warning,
                _ => Strings.Unknown
            };
    }
}
