using System;

namespace RandomizedTesting.Generators
{
    /// <summary>
    /// A <see cref="StringGenerator"/> generates random strings composed of characters. What these characters
    /// are and their distribution depends on a subclass.
    /// </summary>
    public abstract class StringGenerator
    {
        protected const int CharStackBufferSize = 64;

        /// <summary>
        /// An alias for <see cref="OfCodeUnitsLength(Random, int, int)"/>.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> instance to use.</param>
        /// <param name="minCodeUnits">Minimum number of code units (inclusive).</param>
        /// <param name="maxCodeUnits">Maximum number of code units (inclusive).</param>
        /// <returns>A string of variable length between <paramref name="minCodeUnits"/> (inclusive)
        /// and <paramref name="maxCodeUnits"/> (inclusive) length.</returns>
        /// <exception cref="ArgumentException">The generator cannot emit random string
        /// of the given unit length. For example a generator emitting only extended Unicode
        /// plane characters (encoded as surrogate pairs) will not be able to emit an odd number
        /// of code units.</exception>
        /// <exception cref="ArgumentException"><paramref name="minCodeUnits"/> is greater than <paramref name="maxCodeUnits"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minCodeUnits"/> or <paramref name="maxCodeUnits"/> is less than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public virtual string OfStringLength(Random random, int minCodeUnits, int maxCodeUnits)
        {
            return OfCodeUnitsLength(random, minCodeUnits, maxCodeUnits);
        }

        /// <summary>
        /// Returns a string of variable length between <paramref name="minCodeUnits"/> (inclusive)
        /// and <paramref name="maxCodeUnits"/> (inclusive) length. Code units are essentially
        /// an equivalent of <see cref="char"/> type, see <see cref="string"/> class documentation for
        /// explanation.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> instance to use.</param>
        /// <param name="minCodeUnits">Minimum number of code units (inclusive).</param>
        /// <param name="maxCodeUnits">Maximum number of code units (inclusive).</param>
        /// <returns>A string of variable length between <paramref name="minCodeUnits"/> (inclusive)
        /// and <paramref name="maxCodeUnits"/> (inclusive) length.</returns>
        /// <exception cref="ArgumentException">The generator cannot emit random string
        /// of the given unit length. For example a generator emitting only extended Unicode
        /// plane characters (encoded as surrogate pairs) will not be able to emit an odd number
        /// of code units.</exception>
        /// <exception cref="ArgumentException"><paramref name="minCodeUnits"/> is greater than <paramref name="maxCodeUnits"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minCodeUnits"/> or <paramref name="maxCodeUnits"/> is less than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public abstract string OfCodeUnitsLength(Random random, int minCodeUnits, int maxCodeUnits);

        /// <summary>
        /// Returns a string of variable length between <paramref name="minCodePoints"/> (inclusive)
        /// and <paramref name="maxCodePoints"/> (inclusive) length. Code points are full Unicode
        /// codepoints or an equivalent of <see cref="int"/> type, see <a href="https://en.wikipedia.org/wiki/Code_point">Unicode Code Point</a> for
        /// explanation. The returned <see cref="string.Length"/> may exceed <paramref name="maxCodePoints"/>
        /// because certain code points may be encoded as surrogate pairs.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> instance to use.</param>
        /// <param name="minCodePoints">Minimum number of code points (inclusive).</param>
        /// <param name="maxCodePoints">Maximum number of code points (inclusive).</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="minCodePoints"/> is greater than <paramref name="maxCodePoints"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minCodePoints"/> or <paramref name="maxCodePoints"/> is less than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public abstract string OfCodePointsLength(Random random, int minCodePoints, int maxCodePoints);
    }
}
