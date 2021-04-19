using System;

namespace RandomizedTesting.Generators
{
    /// <summary>
    /// Random byte sequence generators.
    /// </summary>
    public static class RandomBytes
    {
        /// <summary>
        /// Returns a byte array with random content.
        /// </summary>
        /// <param name="random">Random generator.</param>
        /// <param name="length">The length of the byte array. Can be zero.</param>
        /// <returns>Returns a byte array with random content.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static byte[] RandomBytesOfLength(Random random, int length)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, $"{nameof(length)} must be greater than or equal to 0.");

            return RandomBytesOfLengthBetween(random, length, length);
        }

        /// <summary>
        /// Returns a byte array with random content.
        /// </summary>
        /// <param name="random">Random generator.</param>
        /// <param name="minLength">The minimum length of the byte array. Can be zero.</param>
        /// <param name="maxLength">The maximum length of the byte array. Can be zero.</param>
        /// <returns>Returns a byte array with random content.</returns>
        /// <exception cref="ArgumentException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> or <paramref name="maxValue"/> is less than zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static byte[] RandomBytesOfLengthBetween(Random random, int minLength, int maxLength)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (minLength < 0)
                throw new ArgumentOutOfRangeException(nameof(minLength), minLength, $"{nameof(minLength)} must be greater than or equal to 0.");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, $"{nameof(maxLength)} must be greater than or equal to 0.");
            if (minLength > maxLength)
                throw new ArgumentException($"{nameof(minLength)} must be less than or equal to {nameof(maxLength)}. {nameof(minLength)}: {minLength}, {nameof(maxLength)}: {maxLength}");

            byte[] bytes = new byte[RandomNumbers.RandomInt32Between(random, minLength, maxLength)];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)random.Next();
            }
            return bytes;
        }
    }
}
