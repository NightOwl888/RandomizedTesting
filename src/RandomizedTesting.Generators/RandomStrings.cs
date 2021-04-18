using System;

namespace RandomizedTesting.Generators
{
    /// <summary>
    /// A facade to various implementations of <see cref="StringGenerator"/> class.
    /// </summary>
    public sealed class RandomStrings
    {
        public readonly static RealisticUnicodeGenerator realisticUnicodeGenerator = new RealisticUnicodeGenerator();
        public readonly static UnicodeGenerator unicodeGenerator = new UnicodeGenerator();
        public readonly static AsciiLettersGenerator asciiLettersGenerator = new AsciiLettersGenerator();
        public readonly static AsciiAlphanumGenerator asciiAlphanumGenerator = new AsciiAlphanumGenerator();

        // NOTE: Didn't port deprected members of the ASCIIGenerator class
        // randomAsciiOfLengthBetween
        // randomAsciiOfLength

        public static string RandomAsciiLettersOfLengthBetween(Random r, int minCodeUnits, int maxCodeUnits) { return asciiLettersGenerator.OfCodeUnitsLength(r, minCodeUnits, maxCodeUnits); }
        public static string RandomAsciiLettersOfLength(Random r, int codeUnits) { return asciiLettersGenerator.OfCodeUnitsLength(r, codeUnits, codeUnits); }

        public static string RandomAsciiAlphanumOfLengthBetween(Random r, int minCodeUnits, int maxCodeUnits) { return asciiAlphanumGenerator.OfCodeUnitsLength(r, minCodeUnits, maxCodeUnits); }
        public static string RandomAsciiAlphanumOfLength(Random r, int codeUnits) { return asciiAlphanumGenerator.OfCodeUnitsLength(r, codeUnits, codeUnits); }

        public static string RandomUnicodeOfLengthBetween(Random r, int minCodeUnits, int maxCodeUnits) { return unicodeGenerator.OfCodeUnitsLength(r, minCodeUnits, maxCodeUnits); }
        public static string RandomUnicodeOfLength(Random r, int codeUnits) { return unicodeGenerator.OfCodeUnitsLength(r, codeUnits, codeUnits); }
        public static string RandomUnicodeOfCodepointLengthBetween(Random r, int minCodePoints, int maxCodePoints) { return unicodeGenerator.OfCodePointsLength(r, minCodePoints, maxCodePoints); }
        public static string RandomUnicodeOfCodepointLength(Random r, int codePoints) { return unicodeGenerator.OfCodePointsLength(r, codePoints, codePoints); }

        public static string RandomRealisticUnicodeOfLengthBetween(Random r, int minCodeUnits, int maxCodeUnits) { return realisticUnicodeGenerator.OfCodeUnitsLength(r, minCodeUnits, maxCodeUnits); }
        public static string RandomRealisticUnicodeOfLength(Random r, int codeUnits) { return realisticUnicodeGenerator.OfCodeUnitsLength(r, codeUnits, codeUnits); }
        public static string RandomRealisticUnicodeOfCodepointLengthBetween(Random r, int minCodePoints, int maxCodePoints) { return realisticUnicodeGenerator.OfCodePointsLength(r, minCodePoints, maxCodePoints); }
        public static string RandomRealisticUnicodeOfCodepointLength(Random r, int codePoints) { return realisticUnicodeGenerator.OfCodePointsLength(r, codePoints, codePoints); }
    }
}
