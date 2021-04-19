using NUnit.Framework;
using System;

namespace RandomizedTesting.Generators
{
    // Stub of RandomizedTest, not full implementation
    [TestFixture]
    public class RandomizedTest
    {
        /// <summary>
        /// Shortcut for the <see cref="Random"/> property of <see cref="TestContext.CurrentContext"/>. Even though this property
        /// is static, it returns a per-thread <see cref="Random"/> instance, so no race conditions can occur.
        /// </summary>
        public static Random Random => TestContext.CurrentContext.Random;

        public static bool RandomBoolean() => Random.NextBoolean();
        public static byte RandomByte() => (byte)Random.Next();
        public static short RandomShort() => (short)Random.Next();
        public static int RandomInt32() => Random.Next();
        public static float RandomSingle() => Random.NextSingle();
        public static double RandomDouble() => Random.NextDouble();
        public static long RandomInt64() => Random.NextInt64();

        //public static double RandomGaussian() => Random.RandomGaussian();

        //
        // Biased value pickers. 
        //

        /**
         * A biased "evil" random float between min and max (inclusive).
         * 
         * @see BiasedNumbers#randomFloatBetween(Random, float, float)
         */
        public static float BiasedSingleBetween(float min, float max) { return BiasedNumbers.RandomSingleBetween(Random, min, max); }

        /**
         * A biased "evil" random double between min and max (inclusive).
         * 
         * @see BiasedNumbers#randomDoubleBetween(Random, double, double)
         */
        public static double BiasedDoubleBetween(double min, double max) { return BiasedNumbers.RandomDoubleBetween(Random, min, max); }

        //
        // Delegates to RandomBytes.
        //

        /** 
         * Returns a byte array with random content.
         * 
         * @param length The length of the byte array. Can be zero.
         * @return Returns a byte array with random content. 
         */
        public static byte[] RandomBytesOfLength(int length)
        {
            return RandomBytes.RandomBytesOfLength(new Random(Random.Next()), length);
        }

        /** 
         * Returns a byte array with random content.
         * 
         * @param minLength The minimum length of the byte array. Can be zero.
         * @param maxLength The maximum length of the byte array. Can be zero.
         * @return Returns a byte array with random content. 
         */
        public static byte[] RandomBytesOfLength(int minLength, int maxLength)
        {
            return RandomBytes.RandomBytesOfLengthBetween(new Random(Random.Next()), minLength, maxLength);
        }

        //
        // Delegates to RandomNumbers.
        //

  //      /** 
  //       * A random integer from 0..max (inclusive). 
  //       */
  //      @Deprecated
  //public static int randomInt(int max)
  //      {
  //          return RandomNumbers.randomIntBetween(getRandom(), 0, max);
  //      }

  //      /** 
  //       * A random long from 0..max (inclusive). 
  //       */
  //      @Deprecated
  //public static long randomLong(long max)
  //      {
  //          return RandomNumbers.randomLongBetween(getRandom(), 0, max);
  //      }

        /** 
         * A random integer from <code>min</code> to <code>max</code> (inclusive).
         * 
         * @see #scaledRandomIntBetween(int, int)
         */
        public static int RandomInt32Between(int min, int max)
        {
            return RandomNumbers.RandomInt32Between(Random, min, max);
        }

        /** 
         * An alias for {@link #randomIntBetween(int, int)}. 
         * 
         * @see #scaledRandomIntBetween(int, int)
         */
        public static int Between(int min, int max)
        {
            return RandomInt32Between(min, max);
        }

        /** 
         * A random long from <code>min</code> to <code>max</code> (inclusive).
         */
        public static long RandomInt64Between(long min, long max)
        {
            return RandomNumbers.RandomInt64Between(Random, min, max);
        }

        /** 
         * An alias for {@link #randomLongBetween}. 
         */
        public static long Between(long min, long max)
        {
            return RandomInt64Between(min, max);
        }

    }
}
