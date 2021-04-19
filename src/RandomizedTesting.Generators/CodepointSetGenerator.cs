using J2N;
using System;

namespace RandomizedTesting.Generators
{
    /// <summary>
    /// A string generator from a predefined set of codepoints or characters.
    /// </summary>
    public class CodepointSetGenerator : StringGenerator
    {
        private readonly int[] bmp;
        private readonly int[] supplementary;
        private readonly int[] all;

        /// <summary>
        /// All characters must be from BMP (no parts of surrogate pairs allowed).
        /// </summary>
        /// <param name="chars"></param>
        public CodepointSetGenerator(char[] chars)
        {
            if (chars is null)
                throw new ArgumentNullException(nameof(chars));

            bmp = new int[chars.Length];
            supplementary = Arrays.Empty<int>();

            for (int i = 0; i < chars.Length; i++)
            {
                bmp[i] = chars[i] & 0xffff;

                if (IsSurrogate(chars[i]))
                {
                    throw new ArgumentException("Value is part of a surrogate pair: 0x"
                        + bmp[i].ToHexString());
                }
            }

            all = Concat(bmp, supplementary);
            if (all.Length == 0)
            {
                throw new ArgumentException("Empty set of characters?");
            }
        }

        /// <summary>
        /// Parse the given <see cref="string"/> <paramref name="value"/> and split into BMP and supplementary codepoints.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public CodepointSetGenerator(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            int bmps = 0;
            int supplementaries = 0;
            for (int i = 0; i < value.Length;)
            {
                int codepoint = value.CodePointAt(i);
                if (Character.IsSupplementaryCodePoint(codepoint))
                {
                    supplementaries++;
                }
                else
                {
                    bmps++;
                }

                i += Character.CharCount(codepoint);
            }

            this.bmp = new int[bmps];
            this.supplementary = new int[supplementaries];
            for (int i = 0; i < value.Length;)
            {
                int codepoint = value.CodePointAt(i);
                if (Character.IsSupplementaryCodePoint(codepoint))
                {
                    supplementary[--supplementaries] = codepoint;
                }
                else
                {
                    bmp[--bmps] = codepoint;
                }

                i += Character.CharCount(codepoint);
            }

            this.all = Concat(bmp, supplementary);
            if (all.Length == 0)
            {
                throw new ArgumentException("Empty set of characters?");
            }
        }

        public override string OfCodeUnitsLength(Random random, int minCodeUnits, int maxCodeUnits)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (minCodeUnits < 0)
                throw new ArgumentOutOfRangeException(nameof(minCodeUnits), $"{nameof(minCodeUnits)} must be greater than or equal to 0.");
            if (maxCodeUnits < 0)
                throw new ArgumentOutOfRangeException(nameof(maxCodeUnits), $"{nameof(maxCodeUnits)} must be greater than or equal to 0.");
            if (minCodeUnits > maxCodeUnits)
                throw new ArgumentException($"{nameof(minCodeUnits)} must be less than or equal to {nameof(maxCodeUnits)}. {nameof(minCodeUnits)}: {minCodeUnits}, {nameof(maxCodeUnits)}: {maxCodeUnits}");

            int length = RandomNumbers.RandomInt32Between(random, minCodeUnits, maxCodeUnits);

            // Check and cater for odd number of code units if no bmp characters are given.
            if (bmp.Length == 0 && IsOdd(length))
            {
                if (minCodeUnits == maxCodeUnits)
                {
                    throw new ArgumentException("Cannot return an odd number of code units "
                        + " when surrogate pairs are the only available codepoints.");
                }
                else
                {
                    // length is odd so we move forward or backward to the closest even number.
                    if (length == minCodeUnits)
                    {
                        length++;
                    }
                    else
                    {
                        length--;
                    }
                }
            }

            int[] codepoints = new int[length];
            int actual = 0;
            while (length > 0)
            {
                if (length == 1)
                {
                    codepoints[actual] = bmp[random.Next(bmp.Length)];
                }
                else
                {
                    codepoints[actual] = all[random.Next(all.Length)];
                }
                if (Character.IsSupplementaryCodePoint(codepoints[actual]))
                {
                    length -= 2;
                }
                else
                {
                    length -= 1;
                }
                actual++;
            }
            return StringUtil.CodePointsToString(codepoints, 0, actual);
        }

        public override string OfCodePointsLength(Random random, int minCodePoints, int maxCodePoints)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            int length = RandomNumbers.RandomInt32Between(random, minCodePoints, maxCodePoints);
            int[] codepoints = new int[length];
            while (length > 0)
            {
                codepoints[--length] = all[random.Next(all.Length)];
            }
            return StringUtil.CodePointsToString(codepoints, 0, codepoints.Length); //new string(codepoints, 0, codepoints.Length);
        }

        /// <summary>
        /// Is a given number odd?
        /// </summary>
        private static bool IsOdd(int v)
        {
            return (v & 1) != 0;
        }

        private int[] Concat(params int[][] arrays)
        {
            int totalLength = 0;
            foreach (int[] a in arrays) totalLength += a.Length;
            int[] concat = new int[totalLength];
            for (int i = 0, j = 0; j < arrays.Length;)
            {
                Array.Copy(arrays[j], 0, concat, i, arrays[j].Length);
                i += arrays[j].Length;
                j++;
            }
            return concat;
        }

        private bool IsSurrogate(char chr)
        {
            return (chr >= 0xd800 && chr <= 0xdfff);
        }
    }
}
