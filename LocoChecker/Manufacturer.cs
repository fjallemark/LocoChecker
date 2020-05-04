using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Tellurian.Trains.Modules
{
    public class Manufacturer
    {
        internal Manufacturer(string line)
        {
            var fields = line.Split(' ');
            Country = fields[^1];
            Number = int.Parse(fields[^2], NumberStyles.Integer, CultureInfo.InvariantCulture);
            Name = string.Join(" ", fields.Take(fields.Length - 4));
        }
        public string Name { get; }
        public int Number { get; }
        public string Country { get; }
    }

    public static class ManufacturerExtensions
    {
        public static IEnumerable<Manufacturer> Read(this string fileName)
        {
            return File.ReadAllLines(fileName).Where(l => l.Length > 5).Select(l => new Manufacturer(l));
        }
    }
}
