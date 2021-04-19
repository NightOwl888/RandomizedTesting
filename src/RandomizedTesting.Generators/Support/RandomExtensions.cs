using J2N;
using J2N.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace RandomizedTesting.Generators
{
    /// <summary>
    /// Extensions for <see cref="Random"/>.
    /// </summary>
    public static class RandomExtensions
    {
        private class RandomProperties
        {
            /// <summary>
            /// The boolean value indicating if the second Gaussian number is available.
            /// </summary>
            internal bool haveNextNextGaussian; // internal for testing

            /// <summary>
            /// The second Gaussian generated number.
            /// </summary>
            internal double nextNextGaussian; // internal for testing
        }

        private static readonly ConditionalWeakTable<Random, RandomProperties> randomCache = new ConditionalWeakTable<Random, RandomProperties>();

        /// <summary>
        /// Pseudo-randomly generates (approximately) a normally distributed
        /// <see cref="double"/> value with mean 0.0 and a standard deviation value
        /// of <c>1.0</c> using the <i>polar method</i> of G. E. P. Box, M.
        /// E. Muller, and G. Marsaglia, as described by Donald E. Knuth in <i>The
        /// Art of Computer Programming, Volume 2: Seminumerical Algorithms</i>,
        /// section 3.4.1, subsection C, algorithm P.
        /// </summary>
        /// <returns>A random <see cref="double"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static double NextGaussian(this Random random)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            var props = randomCache.GetOrCreateValue(random);

            if (props.haveNextNextGaussian)
            { // if X1 has been returned, return the
              // second Gaussian
                props.haveNextNextGaussian = false;
                return props.nextNextGaussian;
            }

            double v1, v2, s;
            do
            {
                v1 = 2 * random.NextDouble() - 1; // Generates two independent random
                                                  // variables U1, U2
                v2 = 2 * random.NextDouble() - 1;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1);
            double norm = Math.Sqrt(-2 * Math.Log(s) / s);
            props.nextNextGaussian = v2 * norm; // should that not be norm instead
                                                // of multiplier ?
            props.haveNextNextGaussian = true;
            return v1 * norm; // should that not be norm instead of multiplier
                              // ?
        }

        /// <summary>
        /// Generates a random <see cref="bool"/>, with a random distribution of
        /// approximately 50/50.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <returns>A random <see cref="bool"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static bool NextBoolean(this Random random)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            return random.Next(1, 100) > 50;
        }

        /// <summary>
        /// Generates a random <see cref="int"/>. <paramref name="minValue"/> and <paramref name="maxValue"/> are BOTH inclusive.
        /// <para/>
        /// NOTE: This differs from <see cref="Random.Next(int, int)"/> in that the end value is inclusive.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="minValue">The inclusive start of the range.</param>
        /// <param name="maxValue">The inclusive end of the range.</param>
        /// <returns>A random <see cref="int"/> from <paramref name="minValue"/> (inclusive) to <paramref name="maxValue"/> (inclusive).</returns>
        /// <exception cref="ArgumentException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> or <paramref name="maxValue"/> is less than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static int NextInt32(this Random random, int minValue, int maxValue)
        {
            return RandomNumbers.RandomInt32Between(random, minValue, maxValue);
        }

        /// <summary>
        /// Generates a random <see cref="long"/>.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <returns>A random <see cref="long"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        // http://stackoverflow.com/a/6651656
        public static long NextInt64(this Random random) // .NET specific to cover missing member from Java
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Generates a random <see cref="long"/> between <c>0</c> (inclusive)
        /// and <paramref name="maxValue"/> (exclusive).
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="maxValue">The bound on the random number to be returned. Must be positive.</param>
        /// <returns>A random <see cref="long"/> between 0 and <paramref name="maxValue"/> - 1.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than 1.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static long NextInt64(this Random random, long maxValue)
        {
            return RandomNumbers.NextInt64(random, maxValue);
        }

        /// <summary>
        /// Generates a random <see cref="long"/>. <paramref name="minValue"/> and <paramref name="maxValue"/> are BOTH inclusive.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="minValue">The inclusive start of the range.</param>
        /// <param name="maxValue">The inclusive end of the range.</param>
        /// <returns>A random <see cref="long"/> from <paramref name="minValue"/> to <paramref name="maxValue"/> (inclusive).</returns>
        /// <exception cref="ArgumentException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static long NextInt64(this Random random, long minValue, long maxValue)
        {
            return RandomNumbers.RandomInt64Between(random, minValue, maxValue);
        }

        /// <summary>
        /// Generates a random <see cref="float"/>.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <returns>A random <see cref="float"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static float NextSingle(this Random random) // .NET specific to cover missing member from Java
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            return (float)random.NextDouble(); // always between 0.0 and 1.0, so it will always cast
        }

        /// <summary>
        /// Generates a random <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="maxBytes">The maximum number of bytes to allocate for storage. Must be greater than 0.</param>
        /// <returns>A random <see cref="BigInteger"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is less than or equal to 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static BigInteger NextBigInteger(this Random random, int maxBytes)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (maxBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxBytes), maxBytes, $"{nameof(maxBytes)} must be greater than 0.");

            int length = RandomNumbers.RandomInt32Between(random, 1, maxBytes);
            byte[] buffer = new byte[length];
            random.NextBytes(buffer);
            return new BigInteger(buffer);
        }

        /// <summary>
        /// Pick a random element from the <paramref name="list"/> (may be an array).
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="list">A list or array to pick a random element from.</param>
        /// <exception cref="ArgumentException"><paramref name="list"/> contains no items.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> or <paramref name="list"/> is <c>null</c>.</exception>
        public static T NextFrom<T>(this Random random, IList<T> list)
        {
            return RandomPicks.RandomFrom(random, list);
        }

        /// <summary>
        /// Pick a random element from the <paramref name="collection"/>.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="collection">A collection to pick a random element from.</param>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains no items.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> or <paramref name="collection"/> is <c>null</c>.</exception>
        public static T NextFrom<T>(this Random random, ICollection<T> collection)
        {
            return RandomPicks.RandomFrom(random, collection);
        }

        /// <summary>
        /// Returns a random string consisting only of lowercase characters 'a' through 'z'. May be an empty string.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="maxLength">The maximum length of the string to return (inclusive).</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextSimpleString(this Random random, int maxLength)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, $"{nameof(maxLength)} must be greater than or equal to 0.");

            return NextSimpleString(random, 0, maxLength);
        }

        /// <summary>
        /// Returns a random string consisting only of lowercase characters 'a' through 'z'. May be an empty string.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="minLength">The minimum length of the string to return (inclusive).</param>
        /// <param name="maxLength">The maximum length of the string to return (inclusive).</param>
        /// <exception cref="ArgumentException"><paramref name="minLength"/> is greater than <paramref name="maxLength"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> or <paramref name="maxLength"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextSimpleString(this Random random, int minLength, int maxLength)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (minLength < 0)
                throw new ArgumentOutOfRangeException(nameof(minLength), minLength, $"{nameof(minLength)} must be greater than or equal to 0.");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, $"{nameof(maxLength)} must be greater than or equal to 0.");
            if (minLength > maxLength)
                throw new ArgumentException($"{nameof(minLength)} must be less than or equal to {nameof(maxLength)}. {nameof(minLength)}: {minLength}, {nameof(maxLength)}: {maxLength}");

            int end = RandomNumbers.RandomInt32Between(random, minLength, maxLength);
            if (end == 0)
            {
                // allow 0 length
                return string.Empty;
            }
            char[] buffer = new char[end];
            for (int i = 0; i < end; i++)
            {
                buffer[i] = (char)RandomNumbers.RandomInt32Between(random, 'a', 'z');
            }
            return new string(buffer, 0, end);
        }

        /// <summary>
        /// Returns a random string consisting only of characters between <paramref name="minChar"/> (inclusive)
        /// and <paramref name="maxChar"/> (inclusive).
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="minChar">The minimum <see cref="char"/> value of the range (inclusive).</param>
        /// <param name="maxChar">The maximum <see cref="char"/> value of the range (inclusive).</param>
        /// <param name="maxLength">The maximum length of the string to generate.</param>
        /// <returns>a random string consisting only of characters between <paramref name="minChar"/> (inclusive)
        /// and <paramref name="maxChar"/> (inclusive).</returns>
        /// <exception cref="ArgumentException"><paramref name="minChar"/> is greater than <paramref name="maxChar"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> or <paramref name="minLength"/> is not in
        /// the range between <see cref="char.MinValue"/> and <see cref="char.MaxValue"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="maxLength"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextSimpleStringRange(this Random random, char minChar, char maxChar, int maxLength)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (minChar < char.MinValue || minChar > char.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(minChar), minChar, $"{nameof(minChar)} must be between {nameof(char.MinValue)} and {nameof(char.MaxValue)}.");
            if (maxChar < char.MinValue || maxChar > char.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(maxChar), maxChar, $"{nameof(maxChar)} must be between {nameof(char.MinValue)} and {nameof(char.MaxValue)}.");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, $"{nameof(maxLength)} must be greater than or equal to 0.");
            if (minChar > maxChar)
                throw new ArgumentException($"{nameof(minChar)} must be less than or equal to {nameof(maxChar)}. {nameof(minChar)}: {minChar}, {nameof(maxChar)}: {maxChar}");

            int end = RandomNumbers.RandomInt32Between(random, 0, maxLength);
            if (end == 0)
            {
                // allow 0 length
                return string.Empty;
            }
            char[] buffer = new char[end];
            for (int i = 0; i < end; i++)
            {
                buffer[i] = (char)RandomNumbers.RandomInt32Between(random, minChar, maxChar);
            }
            return new string(buffer, 0, end);
        }

        /// <summary>
        /// Returns a random string consisting only of lowercase characters 'a' through 'z',
        /// between 0 and 10 characters in length.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextSimpleString(this Random random)
        {
            return NextSimpleString(random, 0, 10);
        }

        /// <summary>
        /// Returns random string with up to 20 characters, including full unicode range.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextUnicodeString(this Random random)
        {
            return NextUnicodeString(random, 20);
        }

        /// <summary>
        /// Returns a random string up to a certain length.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="maxLength">The maximum length of the string to return.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextUnicodeString(this Random random, int maxLength)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, $"{nameof(maxLength)} must be greater than or equal to 0.");

            int end = RandomNumbers.RandomInt32Between(random, 0, maxLength);
            if (end == 0)
            {
                // allow 0 length
                return string.Empty;
            }
            char[] buffer = new char[end];
            NextFixedLengthUnicodeString(random, buffer, 0, buffer.Length);
            return new string(buffer, 0, end);
        }

        /// <summary>
        /// Fills provided <see cref="T:char[]"/> with valid random unicode code
        /// unit sequence.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="chars">A <see cref="T:char[]"/> with preallocated space to put the characters.</param>
        /// <param name="startIndex">The index of <paramref name="chars"/> to begin populating with characters.</param>
        /// <param name="length">The number of characters to populate.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="length"/> refers to a position outside of the range of <paramref name="chars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> or <paramref name="chars"/> is <c>null</c>.</exception>
        public static void NextFixedLengthUnicodeString(this Random random, char[] chars, int startIndex, int length)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (chars is null)
                throw new ArgumentNullException(nameof(chars));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"{nameof(startIndex)} must be greater than or equal to 0.");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), startIndex, $"{nameof(length)} must be greater than or equal to 0.");
            if (startIndex > chars.Length - length)
                throw new ArgumentOutOfRangeException(nameof(length), $"Index and length must refer to a location within the string.");

            int i = startIndex;
            int end = startIndex + length;
            while (i < end)
            {
                int t = random.Next(5);
                if (0 == t && i < length - 1)
                {
                    // Make a surrogate pair
                    // High surrogate
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0xd800, 0xdbff);
                    // Low surrogate
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0xdc00, 0xdfff);
                }
                else if (t <= 1)
                {
                    chars[i++] = (char)random.Next(0x80);
                }
                else if (2 == t)
                {
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0x80, 0x7ff);
                }
                else if (3 == t)
                {
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0x800, 0xd7ff);
                }
                else if (4 == t)
                {
                    chars[i++] = (char)RandomNumbers.RandomInt32Between(random, 0xe000, 0xffff);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> thats "regexish" (contains lots of operators typically found in regular expressions)
        /// If you call this enough times, you might get a valid regex!
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextRegexishString(this Random random)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            return NextRegexishString(random, 20);
        }

        /// <summary>
        /// Maximum recursion bound for '+' and '*' replacements in <see cref="NextRegexishString(Random, int)"/>.
        /// </summary>
        private const int maxRecursionBound = 5;

        /// <summary>
        /// Operators for <see cref="NextRegexishString(Random, int)"/>.
        /// </summary>
        private static readonly IList<string> ops = new [] {
            ".",
            "?",
            "{0," + maxRecursionBound + "}", // bounded replacement for '*'
            "{1," + maxRecursionBound + "}", // bounded replacement for '+'
            "(",
            ")",
            "-",
            "[",
            "]",
            "|"
        };

        /// <summary>
        /// Returns a <see cref="string"/> thats "regexish" (contains lots of operators typically found in regular expressions)
        /// If you call this enough times, you might get a valid regex!
        ///
        /// <para/>Note: to avoid practically endless backtracking patterns we replace asterisk and plus
        /// operators with bounded repetitions. See LUCENE-4111 for more info.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="maxLength"> A hint about maximum length of the regexpish string. It may be exceeded by a few characters. </param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextRegexishString(this Random random, int maxLength)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, $"{nameof(maxLength)} must be greater than or equal to 0.");

            StringBuilder regexp = new StringBuilder(maxLength);
            for (int i = RandomNumbers.RandomInt32Between(random, 0, maxLength); i > 0; i--)
            {
                if (random.NextBoolean())
                {
                    regexp.Append((char)RandomNumbers.RandomInt32Between(random, 'a', 'z'));
                }
                else
                {
                    regexp.Append(RandomPicks.RandomFrom(random, ops));
                }
            }
            return regexp.ToString();
        }

        private static readonly string[] HTML_CHAR_ENTITIES = {
            "AElig",
            "Aacute",
            "Acirc",
            "Agrave",
            "Alpha",
            "AMP",
            "Aring",
            "Atilde",
            "Auml",
            "Beta",
            "COPY",
            "Ccedil",
            "Chi",
            "Dagger",
            "Delta",
            "ETH",
            "Eacute",
            "Ecirc",
            "Egrave",
            "Epsilon",
            "Eta",
            "Euml",
            "Gamma",
            "GT",
            "Iacute",
            "Icirc",
            "Igrave",
            "Iota",
            "Iuml",
            "Kappa",
            "Lambda",
            "LT",
            "Mu",
            "Ntilde",
            "Nu",
            "OElig",
            "Oacute",
            "Ocirc",
            "Ograve",
            "Omega",
            "Omicron",
            "Oslash",
            "Otilde",
            "Ouml",
            "Phi",
            "Pi",
            "Prime",
            "Psi",
            "QUOT",
            "REG",
            "Rho",
            "Scaron",
            "Sigma",
            "THORN",
            "Tau",
            "Theta",
            "Uacute",
            "Ucirc",
            "Ugrave",
            "Upsilon",
            "Uuml",
            "Xi",
            "Yacute",
            "Yuml",
            "Zeta",
            "aacute",
            "acirc",
            "acute",
            "aelig",
            "agrave",
            "alefsym",
            "alpha",
            "amp",
            "and",
            "ang",
            "apos",
            "aring",
            "asymp",
            "atilde",
            "auml",
            "bdquo",
            "beta",
            "brvbar",
            "bull",
            "cap",
            "ccedil",
            "cedil",
            "cent",
            "chi",
            "circ",
            "clubs",
            "cong",
            "copy",
            "crarr",
            "cup",
            "curren",
            "dArr",
            "dagger",
            "darr",
            "deg",
            "delta",
            "diams",
            "divide",
            "eacute",
            "ecirc",
            "egrave",
            "empty",
            "emsp",
            "ensp",
            "epsilon",
            "equiv",
            "eta",
            "eth",
            "euml",
            "euro",
            "exist",
            "fnof",
            "forall",
            "frac12",
            "frac14",
            "frac34",
            "frasl",
            "gamma",
            "ge",
            "gt",
            "hArr",
            "harr",
            "hearts",
            "hellip",
            "iacute",
            "icirc",
            "iexcl",
            "igrave",
            "image",
            "infin",
            "int",
            "iota",
            "iquest",
            "isin",
            "iuml",
            "kappa",
            "lArr",
            "lambda",
            "lang",
            "laquo",
            "larr",
            "lceil",
            "ldquo",
            "le",
            "lfloor",
            "lowast",
            "loz",
            "lrm",
            "lsaquo",
            "lsquo",
            "lt",
            "macr",
            "mdash",
            "micro",
            "middot",
            "minus",
            "mu",
            "nabla",
            "nbsp",
            "ndash",
            "ne",
            "ni",
            "not",
            "notin",
            "nsub",
            "ntilde",
            "nu",
            "oacute",
            "ocirc",
            "oelig",
            "ograve",
            "oline",
            "omega",
            "omicron",
            "oplus",
            "or",
            "ordf",
            "ordm",
            "oslash",
            "otilde",
            "otimes",
            "ouml",
            "para",
            "part",
            "permil",
            "perp",
            "phi",
            "pi",
            "piv",
            "plusmn",
            "pound",
            "prime",
            "prod",
            "prop",
            "psi",
            "quot",
            "rArr",
            "radic",
            "rang",
            "raquo",
            "rarr",
            "rceil",
            "rdquo",
            "real",
            "reg",
            "rfloor",
            "rho",
            "rlm",
            "rsaquo",
            "rsquo",
            "sbquo",
            "scaron",
            "sdot",
            "sect",
            "shy",
            "sigma",
            "sigmaf",
            "sim",
            "spades",
            "sub",
            "sube",
            "sum",
            "sup",
            "sup1",
            "sup2",
            "sup3",
            "supe",
            "szlig",
            "tau",
            "there4",
            "theta",
            "thetasym",
            "thinsp",
            "thorn",
            "tilde",
            "times",
            "trade",
            "uArr",
            "uacute",
            "uarr",
            "ucirc",
            "ugrave",
            "uml",
            "upsih",
            "upsilon",
            "uuml",
            "weierp",
            "xi",
            "yacute",
            "yen",
            "yuml",
            "zeta",
            "zwj",
            "zwnj"
        };

        /// <summary>
        /// Returns a random HTML-like string.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="numElements">The maximum number of HTML elements to include in the string.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numElements"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextHtmlishString(this Random random, int numElements)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (numElements < 0)
                throw new ArgumentOutOfRangeException(nameof(numElements), numElements, $"{nameof(numElements)} must be greater than or equal to 0.");

            int end = RandomNumbers.RandomInt32Between(random, 0, numElements);
            if (end == 0)
            {
                // allow 0 length
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < end; i++)
            {
                int val = random.Next(25);
                switch (val)
                {
                    case 0:
                        sb.Append("<p>");
                        break;
                    case 1:
                        {
                            sb.Append("<");
                            sb.Append("    ".Substring(NextInt32(random, 0, 4)));
                            sb.Append(NextSimpleString(random));
                            for (int j = 0; j < NextInt32(random, 0, 10); ++j)
                            {
                                sb.Append(' ');
                                sb.Append(NextSimpleString(random));
                                sb.Append(" ".Substring(NextInt32(random, 0, 1)));
                                sb.Append('=');
                                sb.Append(" ".Substring(NextInt32(random, 0, 1)));
                                sb.Append("\"".Substring(NextInt32(random, 0, 1)));
                                sb.Append(NextSimpleString(random));
                                sb.Append("\"".Substring(NextInt32(random, 0, 1)));
                            }
                            sb.Append("    ".Substring(NextInt32(random, 0, 4)));
                            sb.Append("/".Substring(NextInt32(random, 0, 1)));
                            sb.Append(">".Substring(NextInt32(random, 0, 1)));
                            break;
                        }
                    case 2:
                        {
                            sb.Append("</");
                            sb.Append("    ".Substring(NextInt32(random, 0, 4)));
                            sb.Append(NextSimpleString(random));
                            sb.Append("    ".Substring(NextInt32(random, 0, 4)));
                            sb.Append(">".Substring(NextInt32(random, 0, 1)));
                            break;
                        }
                    case 3:
                        sb.Append(">");
                        break;
                    case 4:
                        sb.Append("</p>");
                        break;
                    case 5:
                        sb.Append("<!--");
                        break;
                    case 6:
                        sb.Append("<!--#");
                        break;
                    case 7:
                        sb.Append("<script><!-- f('");
                        break;
                    case 8:
                        sb.Append("</script>");
                        break;
                    case 9:
                        sb.Append("<?");
                        break;
                    case 10:
                        sb.Append("?>");
                        break;
                    case 11:
                        sb.Append("\"");
                        break;
                    case 12:
                        sb.Append("\\\"");
                        break;
                    case 13:
                        sb.Append("'");
                        break;
                    case 14:
                        sb.Append("\\'");
                        break;
                    case 15:
                        sb.Append("-->");
                        break;
                    case 16:
                        {
                            sb.Append("&");
                            switch (NextInt32(random, 0, 2))
                            {
                                case 0:
                                    sb.Append(NextSimpleString(random));
                                    break;
                                case 1:
                                    sb.Append(HTML_CHAR_ENTITIES[random.Next(HTML_CHAR_ENTITIES.Length)]);
                                    break;
                            }
                            sb.Append(";".Substring(NextInt32(random, 0, 1)));
                            break;
                        }
                    case 17:
                        {
                            sb.Append("&#");
                            if (0 == NextInt32(random, 0, 1))
                            {
                                sb.Append(NextInt32(random, 0, int.MaxValue - 1));
                                sb.Append(";".Substring(NextInt32(random, 0, 1)));
                            }
                            break;
                        }
                    case 18:
                        {
                            sb.Append("&#x");
                            if (0 == NextInt32(random, 0, 1))
                            {
                                sb.Append(Convert.ToString(NextInt32(random, 0, int.MaxValue - 1), 16));
                                sb.Append(";".Substring(NextInt32(random, 0, 1)));
                            }
                            break;
                        }

                    case 19:
                        sb.Append(";");
                        break;
                    case 20:
                        sb.Append(NextInt32(random, 0, int.MaxValue - 1));
                        break;
                    case 21:
                        sb.Append("\n");
                        break;
                    case 22:
                        sb.Append("          ".Substring(NextInt32(random, 0, 10)));
                        break;
                    case 23:
                        {
                            sb.Append("<");
                            if (0 == NextInt32(random, 0, 3))
                            {
                                sb.Append("          ".Substring(NextInt32(random, 1, 10)));
                            }
                            if (0 == NextInt32(random, 0, 1))
                            {
                                sb.Append("/");
                                if (0 == NextInt32(random, 0, 3))
                                {
                                    sb.Append("          ".Substring(NextInt32(random, 1, 10)));
                                }
                            }
                            switch (NextInt32(random, 0, 3))
                            {
                                case 0:
                                    sb.Append(NextStringRecasing(random, "script"));
                                    break;
                                case 1:
                                    sb.Append(NextStringRecasing(random, "style"));
                                    break;
                                case 2:
                                    sb.Append(NextStringRecasing(random, "br"));
                                    break;
                                    // default: append nothing
                            }
                            sb.Append(">".Substring(NextInt32(random, 0, 1)));
                            break;
                        }
                    default:
                        sb.Append(NextSimpleString(random));
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Randomly upcases, downcases, or leaves intact each code point in the given string in the current culture.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="value">The string to recase randomly.</param>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> or <paramref name="value"/> is <c>null</c>.</exception>
        public static string NextStringRecasing(this Random random, string value)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return NextStringRecasing(random, value, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Randomly upcases, downcases, or leaves intact each code point in the given string in the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="value">The string to recase randomly.</param>
        /// <param name="culture">The culture to use when recasing the string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="random"/>, <paramref name="value"/> or <paramref name="culture"/> is <c>null</c>.</exception>
        public static string NextStringRecasing(this Random random, string value, CultureInfo culture)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (culture is null)
                throw new ArgumentNullException(nameof(culture));

            StringBuilder builder = new StringBuilder();
            int pos = 0;
            while (pos < value.Length)
            {
                int codePoint = value.CodePointAt(pos);
                pos += Character.CharCount(codePoint);
                switch (NextInt32(random, 0, 2))
                {
                    case 0:
                        builder.AppendCodePoint(Character.ToUpper(codePoint, culture));
                        break;
                    case 1:
                        builder.AppendCodePoint(Character.ToLower(codePoint, culture));
                        break;
                    case 2:
                        builder.AppendCodePoint(codePoint); // leave intact
                        break;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns random string of length between 0-20 codepoints, all codepoints within the same unicode block.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextRealisticUnicodeString(this Random random)
        {
            return NextRealisticUnicodeString(random, 20);
        }

        /// <summary>
        /// Returns random string of length up to maxLength codepoints, all codepoints within the same unicode block.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="maxLength">The maximum length of the string to return (inclusive).</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextRealisticUnicodeString(this Random random, int maxLength)
        {
            return NextRealisticUnicodeString(random, 0, maxLength);
        }

        /// <summary>
        /// Returns random string of length between min and max codepoints, all codepoints within the same unicode block.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <param name="minLength">The minimum length of the string to return (inclusive).</param>
        /// <param name="maxLength">The maximum length of the string to return (inclusive).</param>
        /// <exception cref="ArgumentException"><paramref name="minLength"/> is greater than <paramref name="maxLength"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> or <paramref name="maxLength"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextRealisticUnicodeString(this Random random, int minLength, int maxLength)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (minLength < 0)
                throw new ArgumentOutOfRangeException(nameof(minLength), minLength, $"{nameof(minLength)} must be greater than or equal to 0.");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, $"{nameof(maxLength)} must be greater than or equal to 0.");
            if (minLength > maxLength)
                throw new ArgumentException($"{nameof(minLength)} must be less than or equal to {nameof(maxLength)}. {nameof(minLength)}: {minLength}, {nameof(maxLength)}: {maxLength}");

            int end = NextInt32(random, minLength, maxLength);
            int block = random.Next(RealisticUnicodeGenerator.blockStarts.Length);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < end; i++)
                sb.AppendCodePoint(NextInt32(random, RealisticUnicodeGenerator.blockStarts[block], RealisticUnicodeGenerator.blockEnds[block]));
            return sb.ToString();
        }

        /// <summary>
        /// Returns random string, with a given UTF-8 byte <paramref name="length"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static string NextFixedByteLengthUnicodeString(this Random random, int length)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, $"{nameof(length)} must be greater than or equal to 0.");

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
                    buffer[i] = (char)random.Next(0x80);
                    bytes--;
                }
                else if (1 == t)
                {
                    buffer[i] = (char)NextInt32(random, 0x80, 0x7ff);
                    bytes -= 2;
                }
                else if (2 == t)
                {
                    buffer[i] = (char)NextInt32(random, 0x800, 0xd7ff);
                    bytes -= 3;
                }
                else if (3 == t)
                {
                    buffer[i] = (char)NextInt32(random, 0xe000, 0xffff);
                    bytes -= 3;
                }
                else if (4 == t)
                {
                    // Make a surrogate pair
                    // High surrogate
                    buffer[i++] = (char)NextInt32(random, 0xd800, 0xdbff);
                    // Low surrogate
                    buffer[i] = (char)NextInt32(random, 0xdc00, 0xdfff);
                    bytes -= 4;
                }
            }
            return new string(buffer, 0, i);
        }

        /// <summary>
        /// Returns a valid (compiling) <see cref="Regex"/> instance with random stuff inside. Be careful
        /// when applying random patterns to longer strings as certain types of patterns
        /// may explode into exponential times in backtracking implementations (such as .NET's).
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static Regex NextRegex(this Random random)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            const string nonBmpString = "AB\uD840\uDC00C";
            while (true)
            {
                try
                {
                    Regex p = new Regex(NextRegexishString(random), RegexOptions.Compiled);
                    string replacement = p.Replace(nonBmpString, "_");

                    // Make sure the result of applying the pattern to a string with extended
                    // unicode characters is a valid utf16 string. See LUCENE-4078 for discussion.
                    if (replacement != null && ValidUTF16String(replacement))
                    {
                        return p;
                    }
                }
#pragma warning disable 168, IDE0059, CA1031
                catch (Exception ignored)
#pragma warning restore 168, IDE0059, CA1031
                {
                    // Loop trying until we hit something that compiles.
                }
            }
        }

        private static bool ValidUTF16String(string s) // Grabbed from Lucene.NET so we can validate the Regex here
        {
            int size = s.Length;
            for (int i = 0; i < size; i++)
            {
                char ch = s[i];
                if (ch >= Character.MinHighSurrogate && ch <= Character.MaxHighSurrogate)
                {
                    if (i < size - 1)
                    {
                        i++;
                        char nextCH = s[i];
                        if (nextCH >= Character.MinLowSurrogate && nextCH <= Character.MaxLowSurrogate)
                        {
                            // Valid surrogate pair
                        }
                        else
                        // Unmatched high surrogate
                        {
                            return false;
                        }
                    }
                    else
                    // Unmatched high surrogate
                    {
                        return false;
                    }
                }
                else if (ch >= Character.MinLowSurrogate && ch <= Character.MaxLowSurrogate)
                // Unmatched low surrogate
                {
                    return false;
                }
            }

            return true;
        }

        // Not implemented here - leave in Lucene.NET (too specialized)
        // FilteredQuery.FilterStrategy NextFilterStrategy(this Random random)
        // string NextWhitespace(this Random random, int minLength, int maxLength)
        // string NextAnalysisString(this Random random, int maxLength, bool simple)
        // string NextSubString(this Random random, int wordLength, bool simple)
    }
}
