using J2N;
using System;
using System.Diagnostics;
using System.Globalization;

namespace RandomizedTesting.Generators
{
    internal ref partial struct ValueStringBuilder
    {
        public void AppendNextSimpleString(Random random)
            => AppendNextSimpleString(random, minLength: 0, maxLength: 10);

        public void AppendNextSimpleString(Random random, int maxLength)
            => AppendNextSimpleString(random, minLength: 0, maxLength);

        public void AppendNextSimpleString(Random random, int minLength, int maxLength)
        {
            Debug.Assert(random != null);

            int end = RandomNumbers.RandomInt32Between(random!, minLength, maxLength);
            if (end == 0) return; // allow 0 length
            AppendNextSimpleStringInternal(random!, end);
        }

        internal void AppendNextSimpleStringInternal(Random random, int end)
        {
            Debug.Assert(random != null);

            for (int i = 0; i < end; i++)
            {
                Append((char)RandomNumbers.RandomInt32Between(random!, 'a', 'z'));
            }
        }

        public void AppendNextStringRecasing(Random random, string value)
            => AppendNextStringRecasing(random, value, CultureInfo.CurrentCulture);

        public void AppendNextStringRecasing(Random random, string value, CultureInfo culture)
        {
            Debug.Assert(random != null);
            Debug.Assert(value != null);

            int pos = 0;
            while (pos < value!.Length)
            {
                int codePoint = value.CodePointAt(pos);
                pos += Character.CharCount(codePoint);
                switch (RandomNumbers.RandomInt32Between(random!, 0, 2))
                {
                    case 0:
                        AppendCodePoint(Character.ToUpper(codePoint, culture));
                        break;
                    case 1:
                        AppendCodePoint(Character.ToLower(codePoint, culture));
                        break;
                    case 2:
                        AppendCodePoint(codePoint); // leave intact
                        break;
                }
            }
        }

        public void AppendNextFixedLengthUnicodeString(Random random, int length)
        {
            int i = 0;
            while (i < length)
            {
                int t = random.Next(5);
                if (0 == t && i < length - 1)
                {
                    // Make a surrogate pair
                    // High surrogate
                    Append((char)RandomNumbers.RandomInt32Between(random, 0xd800, 0xdbff));
                    i++;
                    // Low surrogate
                    Append((char)RandomNumbers.RandomInt32Between(random, 0xdc00, 0xdfff));
                    i++;
                }
                else if (t <= 1)
                {
                    Append((char)random.Next(0x80));
                    i++;
                }
                else if (2 == t)
                {
                    Append((char)RandomNumbers.RandomInt32Between(random, 0x80, 0x7ff));
                    i++;
                }
                else if (3 == t)
                {
                    Append((char)RandomNumbers.RandomInt32Between(random, 0x800, 0xd7ff));
                    i++;
                }
                else if (4 == t)
                {
                    Append((char)RandomNumbers.RandomInt32Between(random, 0xe000, 0xffff));
                    i++;
                }
            }
        }
    }
}
