using System;

namespace Qweree.WebApplication.Infrastructure.View.BinarySize
{
    public readonly struct BinaryUnit
    {
        public const string ValueFormat = "# ##0.###";

        public static readonly BinaryUnit Byte = new("B", "Byte", 0);
        public static readonly BinaryUnit KiloByte = new("KB", "Kilobyte", 3);
        public static readonly BinaryUnit MegaByte = new("MB", "Megabyte", 6);
        public static readonly BinaryUnit GigaByte = new("GB", "Gigabyte", 9);
        public static readonly BinaryUnit Terabyte = new("TB", "Terabyte", 12);
        public static readonly BinaryUnit Petabyte = new("PB", "Petabyte", 15);

        public static BinarySize Convert(BinarySize binarySize, BinaryUnit to)
        {
            var value = binarySize.Value;
            var from = binarySize.Unit;

            value *= Math.Pow(10, from.Exponent);
            value /= Math.Pow(10, to.Exponent);

            return new BinarySize(value, to);
        }

        public static BinarySize GetUserSize(BinarySize binarySize)
        {
            var bytes = binarySize.Value;

            if (bytes < Math.Pow(10, 3))
                return binarySize;
            if (bytes < Math.Pow(10, 6))
                return Convert(binarySize, KiloByte);
            if (bytes <  Math.Pow(10, 9))
                return Convert(binarySize, MegaByte);
            if (bytes <  Math.Pow(10, 12))
                return Convert(binarySize, GigaByte);
            if (bytes <  Math.Pow(10, 15))
                return Convert(binarySize, Terabyte);

            return Convert(binarySize, Petabyte);
        }

        public BinaryUnit(string shortName, string name, int exponent)
        {
            ShortName = shortName;
            Name = name;
            Exponent = exponent;
        }

        public string ShortName { get; }
        public string Name { get; }
        public int Exponent { get; }
    }
}