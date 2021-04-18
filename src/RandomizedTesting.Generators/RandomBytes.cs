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
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static byte[] RandomBytesOfLength(Random random, int length)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            return RandomBytesOfLengthBetween(random, length, length);
        }

        /// <summary>
        /// Returns a byte array with random content.
        /// </summary>
        /// <param name="random">Random generator.</param>
        /// <param name="minLength">The minimum length of the byte array. Can be zero.</param>
        /// <param name="maxLength">The maximum length of the byte array. Can be zero.</param>
        /// <returns>Returns a byte array with random content.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static byte[] RandomBytesOfLengthBetween(Random random, int minLength, int maxLength)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));

            byte[] bytes = new byte[RandomNumbers.RandomInt32Between(random, minLength, maxLength)];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)random.Next();
            }
            return bytes;
        }
    }
}
