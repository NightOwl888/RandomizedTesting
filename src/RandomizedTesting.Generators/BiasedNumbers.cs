using J2N;
using System;
using System.Diagnostics;

namespace RandomizedTesting.Generators
{
    /// <summary>
    /// Utility for selecting numbers at random, but not necessarily
    /// in a uniform way. The implementation will try to pick "evil" numbers
    /// more often than uniform selection would. This includes exact range
    /// boundaries, numbers very close to range boundaries, numbers very close
    /// (or equal) to zero, etc.
    /// <para/>
    /// The exact method of selection is implementation-dependent and
    /// may change (if we find even more evil ways).
    /// </summary>
    public sealed class BiasedNumbers
    {
        private const int EvilRangeLeft = 1;
        private const int EvilRangeRight = 1;
        private const int EvilVeryCloseRangeEnds = 20;
        private const int EvilZeroOrNear = 5;
        private const int EvilSimpleProportion = 10;
        private const int EvilRandomRepresentationBits = 10;

        /// <summary>
        /// A random <see cref="double"/> between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/>
        /// (inclusive). If you wish to have an exclusive range,
        /// use <see cref="MathExtensions.NextAfter(float, double)"/> to adjust the range.
        /// <para/>
        /// The code was inspired by GeoTestUtil from Apache Lucene.
        /// </summary>
        /// <param name="random">The <see cref="Random"/> instance.</param>
        /// <param name="minValue">Left range boundary, inclusive. May be <see cref="double.NegativeInfinity"/>, but not <see cref="double.NaN"/>.</param>
        /// <param name="maxValue">Right range boundary, inclusive. May be <see cref="double.PositiveInfinity"/>, but not <see cref="double.NaN"/>.</param>
        /// <returns>A random <see cref="double"/> between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/>
        /// (inclusive).</returns>
        /// <exception cref="ArgumentException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> or <paramref name="maxValue"/> is <see cref="double.NaN"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static double RandomDoubleBetween(Random random, double minValue, double maxValue)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (maxValue < minValue)
                throw new ArgumentException($"max must be >= min: {minValue}, {maxValue}");
            if (double.IsNaN(minValue))
                throw new ArgumentOutOfRangeException(nameof(minValue), "min must not be NaN");
            if (double.IsNaN(maxValue))
                throw new ArgumentOutOfRangeException(nameof(maxValue), "max must not be NaN");

            bool hasZero = minValue <= 0 && maxValue >= 0;

            int pick = random.Next(
                EvilRangeLeft +
                EvilRangeRight +
                EvilVeryCloseRangeEnds +
                (hasZero ? EvilZeroOrNear : 0) +
                EvilSimpleProportion +
                EvilRandomRepresentationBits);

            // Exact range ends
            pick -= EvilRangeLeft;
            if (pick < 0 || minValue == maxValue)
            {
                return minValue;
            }

            pick -= EvilRangeRight;
            if (pick < 0)
            {
                return maxValue;
            }

            // If we're dealing with infinities, adjust them to discrete values.
            Debug.Assert(minValue != maxValue);
            if (double.IsInfinity(minValue))
            {
                minValue = minValue.NextUp();
            }
            if (double.IsInfinity(maxValue))
            {
                maxValue = maxValue.NextAfter(double.NegativeInfinity);
            }

            // Numbers "very" close to range ends. "very" means a few floating point 
            // representation steps (ulps) away.
            pick -= EvilVeryCloseRangeEnds;
            if (pick < 0)
            {
                if (random.NextBoolean())
                {
                    return FuzzUp(random, minValue, maxValue);
                }
                else
                {
                    return FuzzDown(random, maxValue, minValue);
                }
            }

            // Zero or near-zero values, if within the range.
            if (hasZero)
            {
                pick -= EvilZeroOrNear;
                if (pick < 0)
                {
                    int v = random.Next(4);
                    if (v == 0)
                    {
                        return 0d;
                    }
                    else if (v == 1)
                    {
                        return -0.0d;
                    }
                    else if (v == 2)
                    {
                        return FuzzDown(random, 0d, minValue);
                    }
                    else if (v == 3)
                    {
                        return FuzzUp(random, 0d, maxValue);
                    }
                }
            }

            // Simple proportional selection.
            pick -= EvilSimpleProportion;
            if (pick < 0)
            {
                return minValue + (maxValue - minValue) * random.NextDouble();
            }

            // Random representation space selection. This will be heavily biased
            // and overselect from the set of tiny values, if they're allowed.
            pick -= EvilRandomRepresentationBits;
            if (pick < 0)
            {
                long from = ToSortable(minValue);
                long to = ToSortable(maxValue);
                return FromSortable(RandomNumbers.RandomInt64Between(random, from, to));
            }

            throw new Exception("Unreachable.");
        }

        /// <summary>
        /// Fuzzify the input value by decreasing it by a few ulps, but never past <paramref name="minValue"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static double FuzzDown(Random random, double value, double minValue)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (value < minValue)
                throw new ArgumentException("value must be >= min");

            for (int steps = RandomNumbers.RandomInt32Between(random, 1, 10); steps > 0 && value > minValue; steps--)
            {
                value = value.NextAfter(double.NegativeInfinity);
            }
            return value;
        }

        /// <summary>
        /// Fuzzify the input value by increasing it by a few ulps, but never past <paramref name="maxValue"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static double FuzzUp(Random random, double value, double maxValue)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (value > maxValue)
                throw new ArgumentException($"{nameof(value)} must be <= {nameof(maxValue)}");

            for (int steps = RandomNumbers.RandomInt32Between(random, 1, 10); steps > 0 && value < maxValue; steps--)
            {
                value = value.NextUp();
            }
            return value;
        }

        private static double FromSortable(long sortable)
        {
            return BitConversion.Int64BitsToDouble(Flip(sortable));
        }

        private static long ToSortable(double value)
        {
            return Flip(BitConversion.DoubleToInt64Bits(value));
        }

        private static long Flip(long bits)
        {
            return bits ^ (bits >> 63) & 0x7fffffffffffffffL;
        }

        /// <summary>
        /// A random <see cref="float"/> between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/>
        /// (inclusive). If you wish to have an exclusive range,
        /// use <see cref="MathExtensions.NextAfter(float, double)"/> to adjust the range.
        /// <para/>
        /// The code was inspired by GeoTestUtil from Apache Lucene.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="minValue">Left range boundary, inclusive. May be <see cref="float.NegativeInfinity"/>, but not <see cref="float.NaN"/>.</param>
        /// <param name="maxValue">Right range boundary, inclusive. May be <see cref="float.PositiveInfinity"/>, but not <see cref="float.NaN"/>.</param>
        /// <returns>A random <see cref="float"/> between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/>
        /// (inclusive).</returns>
        /// <exception cref="ArgumentException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> or <paramref name="maxValue"/> is <see cref="double.NaN"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static float RandomSingleBetween(Random random, float minValue, float maxValue)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (maxValue < minValue)
                throw new ArgumentException($"{nameof(minValue)} must be >= {nameof(minValue)}: {minValue}, {maxValue}");
            if (float.IsNaN(minValue))
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} must not be NaN");
            if (float.IsNaN(maxValue))
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must not be NaN");

            bool hasZero = minValue <= 0 && maxValue >= 0;

            int pick = random.Next(
                EvilRangeLeft +
                EvilRangeRight +
                EvilVeryCloseRangeEnds +
                (hasZero ? EvilZeroOrNear : 0) +
                EvilSimpleProportion +
                EvilRandomRepresentationBits);

            // Exact range ends
            pick -= EvilRangeLeft;
            if (pick < 0 || minValue == maxValue)
            {
                return minValue;
            }

            pick -= EvilRangeRight;
            if (pick < 0)
            {
                return maxValue;
            }

            // If we're dealing with infinities, adjust them to discrete values.
            Debug.Assert(minValue != maxValue);
            if (float.IsInfinity(minValue))
            {
                minValue = minValue.NextUp();
            }
            if (float.IsInfinity(maxValue))
            {
                maxValue = maxValue.NextAfter(double.NegativeInfinity);
            }

            // Numbers "very" close to range ends. "very" means a few floating point 
            // representation steps (ulps) away.
            pick -= EvilVeryCloseRangeEnds;
            if (pick < 0)
            {
                if (random.NextBoolean())
                {
                    return FuzzUp(random, minValue, maxValue);
                }
                else
                {
                    return FuzzDown(random, maxValue, minValue);
                }
            }

            // Zero or near-zero values, if within the range.
            if (hasZero)
            {
                pick -= EvilZeroOrNear;
                if (pick < 0)
                {
                    int v = random.Next(4);
                    if (v == 0)
                    {
                        return 0f;
                    }
                    else if (v == 1)
                    {
                        return -0.0f;
                    }
                    else if (v == 2)
                    {
                        return FuzzDown(random, 0f, minValue);
                    }
                    else if (v == 3)
                    {
                        return FuzzUp(random, 0f, maxValue);
                    }
                }
            }

            // Simple proportional selection.
            pick -= EvilSimpleProportion;
            if (pick < 0)
            {
                return (float)(minValue + (((double)maxValue - minValue) * random.NextDouble()));
            }

            // Random representation space selection. This will be heavily biased
            // and overselect from the set of tiny values, if they're allowed.
            pick -= EvilRandomRepresentationBits;
            if (pick < 0)
            {
                int from = ToSortable(minValue);
                int to = ToSortable(maxValue);
                return FromSortable(RandomNumbers.RandomInt32Between(random, from, to));
            }

            throw new Exception("Unreachable.");
        }

        /// <summary>
        /// Fuzzify the input value by decreasing it by a few ulps, but never past <paramref name="minValue"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static float FuzzDown(Random random, float value, float minValue)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (value < minValue)
                throw new ArgumentException($"{nameof(value)} must be >= {nameof(minValue)}");

            for (int steps = RandomNumbers.RandomInt32Between(random, 1, 10); steps > 0 && value > minValue; steps--)
            {
                value = value.NextAfter(double.NegativeInfinity);
            }
            return value;
        }

        /// <summary>
        /// Fuzzify the input value by increasing it by a few ulps, but never past <paramref name="maxValue"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
        public static float FuzzUp(Random random, float value, float maxValue)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (value > maxValue)
                throw new ArgumentException($"{nameof(value)} must be <= {nameof(maxValue)}");

            for (int steps = RandomNumbers.RandomInt32Between(random, 1, 10); steps > 0 && value < maxValue; steps--)
            {
                value = value.NextUp();
            }
            return value;
        }

        private static float FromSortable(int sortable)
        {
            return BitConversion.Int32BitsToSingle(Flip(sortable));
        }

        private static int ToSortable(float value)
        {
            return Flip(BitConversion.SingleToInt32Bits(value));
        }

        private static int Flip(int floatBits)
        {
            return floatBits ^ (floatBits >> 31) & 0x7fffffff;
        }
    }
}
