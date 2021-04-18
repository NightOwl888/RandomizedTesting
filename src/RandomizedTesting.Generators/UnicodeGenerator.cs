using J2N;
using System;

namespace RandomizedTesting.Generators
{
    /// <summary>
    /// A string generator that emits valid Unicode codepoints.
    /// </summary>
    public class UnicodeGenerator : StringGenerator
    {
        private const int SurrogateRange = Character.MaxSurrogate - Character.MinSurrogate + 1;
        private const int CodePointRange = Character.MaxCodePoint - SurrogateRange;

        /// <inheritdoc/>
        public override string OfCodeUnitsLength(Random random, int minCodeUnits, int maxCodeUnits)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (minCodeUnits < 0)
                throw new ArgumentOutOfRangeException(nameof(minCodeUnits), minCodeUnits, $"{nameof(minCodeUnits)} must be greater than or equal to 0.");
            if (maxCodeUnits < 0)
                throw new ArgumentOutOfRangeException(nameof(maxCodeUnits), maxCodeUnits, $"{nameof(maxCodeUnits)} must be greater than or equal to 0.");
            if (minCodeUnits > maxCodeUnits)
                throw new ArgumentException($"{nameof(minCodeUnits)} must be less than or equal {nameof(maxCodeUnits)}. {nameof(minCodeUnits)}: {minCodeUnits}, {nameof(maxCodeUnits)}: {maxCodeUnits}");

            int length = RandomNumbers.RandomInt32Between(random, minCodeUnits, maxCodeUnits);
            char[] chars = new char[length];
            for (int i = 0; i < chars.Length;)
            {
                int t = RandomNumbers.RandomInt32Between(random, 0, 4);
                if (t == 0 && i < length - 1)
                {
                    // Make a surrogate pair
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0xd800, 0xdbff); // high
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0xdc00, 0xdfff); // low
                }
                else if (t <= 1)
                {
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0, 0x007f);
                }
                else if (t == 2)
                {
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0x80, 0x07ff);
                }
                else if (t == 3)
                {
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0x800, 0xd7ff);
                }
                else if (t == 4)
                {
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0xe000, 0xffff);
                }
            }
            return new string(chars);
        }

        /// <inheritdoc/>
        public override string OfCodePointsLength(Random random, int minCodePoints, int maxCodePoints)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (minCodePoints < 0)
                throw new ArgumentOutOfRangeException(nameof(minCodePoints), minCodePoints, $"{nameof(minCodePoints)} must be greater than or equal to 0.");
            if (maxCodePoints < 0)
                throw new ArgumentOutOfRangeException(nameof(maxCodePoints), maxCodePoints, $"{nameof(maxCodePoints)} must be greater than or equal to 0.");
            if (minCodePoints > maxCodePoints)
                throw new ArgumentException($"{nameof(minCodePoints)} must be less than or equal {nameof(maxCodePoints)}. {nameof(minCodePoints)}: {minCodePoints}, {nameof(maxCodePoints)}: {maxCodePoints}");

            int length = RandomNumbers.RandomInt32Between(random, minCodePoints, maxCodePoints);
            int[] chars = new int[length];
            for (int i = 0; i < chars.Length; i++)
            {
                int v = RandomNumbers.RandomInt32Between(random, 0, CodePointRange);
                if (v >= Character.MinSurrogate)
                    v += SurrogateRange;
                chars[i] = v;
            }
            return StringUtil.CodePointsToString(chars, 0, chars.Length); //new string(chars, 0, chars.Length);
        }

        /// <summary>
        /// Returns a random string that will have a random UTF-8 representation length between
        /// <paramref name="minUtf8Length"/> and <paramref name="maxUtf8Length"/>.
        /// </summary>
        /// <param name="random">A <see cref="Random"/> instance.</param>
        /// <param name="minUtf8Length">Minimum UTF-8 representation length (inclusive).</param>
        /// <param name="maxUtf8Length">Maximum UTF-8 representation length (inclusive).</param>
        /// <returns>A random string that will have a random UTF-8 representation length between
        /// <paramref name="minUtf8Length"/> and <paramref name="maxUtf8Length"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="minUtf8Length"/> is greater than <paramref name="maxUtf8Length"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minUtf8Length"/> or <paramref name="maxUtf8Length"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public string OfUtf8Length(Random random, int minUtf8Length, int maxUtf8Length)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (minUtf8Length < 0)
                throw new ArgumentOutOfRangeException(nameof(minUtf8Length), minUtf8Length, $"{nameof(minUtf8Length)} must be greater than or equal to 0.");
            if (maxUtf8Length < 0)
                throw new ArgumentOutOfRangeException(nameof(maxUtf8Length), maxUtf8Length, $"{nameof(maxUtf8Length)} must be greater than or equal to 0.");
            if (minUtf8Length > maxUtf8Length)
                throw new ArgumentException($"{nameof(minUtf8Length)} must be less than or equal {nameof(maxUtf8Length)}. {nameof(minUtf8Length)}: {minUtf8Length}, {nameof(maxUtf8Length)}: {maxUtf8Length}");

            int length = RandomNumbers.RandomInt32Between(random, minUtf8Length, maxUtf8Length);
            char[] buffer = new char[length * 3];
            int bytes = length;
            int i = 0;
            for (; i < buffer.Length && bytes != 0; i++)
            {
                int t;
                if (bytes >= 4)
                {
                    t = random.Next(5);
                }
                else if (bytes >= 3)
                {
                    t = random.Next(4);
                }
                else if (bytes >= 2)
                {
                    t = random.Next(2);
                }
                else
                {
                    t = 0;
                }
                if (t == 0)
                {
                    buffer[i] = (char)RandomNumbers.RandomInt32Between(random, 0, 0x7f);
                    bytes--;
                }
                else if (1 == t)
                {
                    buffer[i] = (char)RandomNumbers.RandomInt32Between(random, 0x80, 0x7ff);
                    bytes -= 2;
                }
                else if (2 == t)
                {
                    buffer[i] = (char)RandomNumbers.RandomInt32Between(random, 0x800, 0xd7ff);
                    bytes -= 3;
                }
                else if (3 == t)
                {
                    buffer[i] = (char)RandomNumbers.RandomInt32Between(random, 0xe000, 0xffff);
                    bytes -= 3;
                }
                else if (4 == t)
                {
                    // Make a surrogate pair
                    buffer[i++] = (char)RandomNumbers.RandomInt32Between(random, 0xd800, 0xdbff); // high
                    buffer[i] = (char)RandomNumbers.RandomInt32Between(random, 0xdc00, 0xdfff);   // low
                    bytes -= 4;
                }
            }
            return new string(buffer, 0, i);
        }
    }
}
