using J2N;
using System.Diagnostics;

namespace RandomizedTesting.Generators
{
    // TODO: Move these to J2N.MathExtensions + tests
    internal static class MathExtensions
    {
        /// <summary>
        /// Bit mask to isolate the sign bit of a <see cref="float"/>.
        /// </summary>
        private const int SingleSignBitMask = unchecked((int)0x80000000);

        /// <summary>
        /// Bit mask to isolate the sign bit of a <see cref="double"/>.
        /// </summary>
        private const long DoubleSignBitMask = unchecked((long)0x8000000000000000L);

        /**
         * Returns the floating-point number adjacent to the first
         * argument in the direction of the second argument.  If both
         * arguments compare as equal the second argument is returned.
         *
         * <p>
         * Special cases:
         * <ul>
         * <li> If either argument is a NaN, then NaN is returned.
         *
         * <li> If both arguments are signed zeros, {@code direction}
         * is returned unchanged (as implied by the requirement of
         * returning the second argument if the arguments compare as
         * equal).
         *
         * <li> If {@code start} is
         * &plusmn;{@link Double#MIN_VALUE} and {@code direction}
         * has a value such that the result should have a smaller
         * magnitude, then a zero with the same sign as {@code start}
         * is returned.
         *
         * <li> If {@code start} is infinite and
         * {@code direction} has a value such that the result should
         * have a smaller magnitude, {@link Double#MAX_VALUE} with the
         * same sign as {@code start} is returned.
         *
         * <li> If {@code start} is equal to &plusmn;
         * {@link Double#MAX_VALUE} and {@code direction} has a
         * value such that the result should have a larger magnitude, an
         * infinity with same sign as {@code start} is returned.
         * </ul>
         *
         * @param start  starting floating-point value
         * @param direction value indicating which of
         * {@code start}'s neighbors or {@code start} should
         * be returned
         * @return The floating-point number adjacent to {@code start} in the
         * direction of {@code direction}.
         * @since 1.6
         */
        public static double NextAfter(this double start, double direction)
        {
            /*
             * The cases:
             *
             * nextAfter(+infinity, 0)  == MAX_VALUE
             * nextAfter(+infinity, +infinity)  == +infinity
             * nextAfter(-infinity, 0)  == -MAX_VALUE
             * nextAfter(-infinity, -infinity)  == -infinity
             *
             * are naturally handled without any additional testing
             */

            // First check for NaN values
            if (double.IsNaN(start) || double.IsNaN(direction))
            {
                // return a NaN derived from the input NaN(s)
                return start + direction;
            }
            else if (start == direction)
            {
                return direction;
            }
            else
            {        // start > direction or start < direction
                     // Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0)
                     // then bitwise convert start to integer.
                long transducer = BitConversion.DoubleToRawInt64Bits(start + 0.0d);

                /*
                 * IEEE 754 floating-point numbers are lexicographically
                 * ordered if treated as signed- magnitude integers .
                 * Since Java's integers are two's complement,
                 * incrementing" the two's complement representation of a
                 * logically negative floating-point value *decrements*
                 * the signed-magnitude representation. Therefore, when
                 * the integer representation of a floating-point values
                 * is less than zero, the adjustment to the representation
                 * is in the opposite direction than would be expected at
                 * first .
                 */
                if (direction > start)
                { // Calculate next greater value
                    transducer = transducer + (transducer >= 0L ? 1L : -1L);
                }
                else
                { // Calculate next lesser value
                    Debug.Assert(direction < start);
                    if (transducer > 0L)
                        --transducer;
                    else
                        if (transducer < 0L)
                        ++transducer;
                    /*
                     * transducer==0, the result is -MIN_VALUE
                     *
                     * The transition from zero (implicitly
                     * positive) to the smallest negative
                     * signed magnitude value must be done
                     * explicitly.
                     */
                    else
                        transducer = DoubleSignBitMask | 1L;
                }

                return BitConversion.Int64BitsToDouble(transducer);
            }
        }

        /**
         * Returns the floating-point number adjacent to the first
         * argument in the direction of the second argument.  If both
         * arguments compare as equal a value equivalent to the second argument
         * is returned.
         *
         * <p>
         * Special cases:
         * <ul>
         * <li> If either argument is a NaN, then NaN is returned.
         *
         * <li> If both arguments are signed zeros, a value equivalent
         * to {@code direction} is returned.
         *
         * <li> If {@code start} is
         * &plusmn;{@link Float#MIN_VALUE} and {@code direction}
         * has a value such that the result should have a smaller
         * magnitude, then a zero with the same sign as {@code start}
         * is returned.
         *
         * <li> If {@code start} is infinite and
         * {@code direction} has a value such that the result should
         * have a smaller magnitude, {@link Float#MAX_VALUE} with the
         * same sign as {@code start} is returned.
         *
         * <li> If {@code start} is equal to &plusmn;
         * {@link Float#MAX_VALUE} and {@code direction} has a
         * value such that the result should have a larger magnitude, an
         * infinity with same sign as {@code start} is returned.
         * </ul>
         *
         * @param start  starting floating-point value
         * @param direction value indicating which of
         * {@code start}'s neighbors or {@code start} should
         * be returned
         * @return The floating-point number adjacent to {@code start} in the
         * direction of {@code direction}.
         * @since 1.6
         */
        public static float NextAfter(this float start, double direction)
        {
            /*
             * The cases:
             *
             * nextAfter(+infinity, 0)  == MAX_VALUE
             * nextAfter(+infinity, +infinity)  == +infinity
             * nextAfter(-infinity, 0)  == -MAX_VALUE
             * nextAfter(-infinity, -infinity)  == -infinity
             *
             * are naturally handled without any additional testing
             */

            // First check for NaN values
            if (float.IsNaN(start) || double.IsNaN(direction))
            {
                // return a NaN derived from the input NaN(s)
                return start + (float)direction;
            }
            else if (start == direction)
            {
                return (float)direction;
            }
            else
            {        // start > direction or start < direction
                     // Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0)
                     // then bitwise convert start to integer.
                int transducer = BitConversion.SingleToRawInt32Bits(start + 0.0f);

                /*
                 * IEEE 754 floating-point numbers are lexicographically
                 * ordered if treated as signed- magnitude integers .
                 * Since Java's integers are two's complement,
                 * incrementing" the two's complement representation of a
                 * logically negative floating-point value *decrements*
                 * the signed-magnitude representation. Therefore, when
                 * the integer representation of a floating-point values
                 * is less than zero, the adjustment to the representation
                 * is in the opposite direction than would be expected at
                 * first.
                 */
                if (direction > start)
                {// Calculate next greater value
                    transducer = transducer + (transducer >= 0 ? 1 : -1);
                }
                else
                { // Calculate next lesser value
                    Debug.Assert(direction < start);
                    if (transducer > 0)
                        --transducer;
                    else
                        if (transducer < 0)
                        ++transducer;
                    /*
                     * transducer==0, the result is -MIN_VALUE
                     *
                     * The transition from zero (implicitly
                     * positive) to the smallest negative
                     * signed magnitude value must be done
                     * explicitly.
                     */
                    else
                        transducer = SingleSignBitMask | 1;
                }

                return BitConversion.Int32BitsToSingle(transducer);
            }
        }

        /**
         * Returns the floating-point value adjacent to {@code d} in
         * the direction of positive infinity.  This method is
         * semantically equivalent to {@code nextAfter(d,
         * Double.POSITIVE_INFINITY)}; however, a {@code nextUp}
         * implementation may run faster than its equivalent
         * {@code nextAfter} call.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, the result is NaN.
         *
         * <li> If the argument is positive infinity, the result is
         * positive infinity.
         *
         * <li> If the argument is zero, the result is
         * {@link Double#MIN_VALUE}
         *
         * </ul>
         *
         * @param d starting floating-point value
         * @return The adjacent floating-point value closer to positive
         * infinity.
         * @since 1.6
         */
        public static double NextUp(this double d)
        {
            if (double.IsNaN(d) || d == double.PositiveInfinity)
                return d;
            else
            {
                d += 0.0d;
                return BitConversion.Int64BitsToDouble(BitConversion.DoubleToRawInt64Bits(d) +
                                               ((d >= 0.0d) ? +1L : -1L));
            }
        }

        /**
         * Returns the floating-point value adjacent to {@code f} in
         * the direction of positive infinity.  This method is
         * semantically equivalent to {@code nextAfter(f,
         * Float.POSITIVE_INFINITY)}; however, a {@code nextUp}
         * implementation may run faster than its equivalent
         * {@code nextAfter} call.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, the result is NaN.
         *
         * <li> If the argument is positive infinity, the result is
         * positive infinity.
         *
         * <li> If the argument is zero, the result is
         * {@link Float#MIN_VALUE}
         *
         * </ul>
         *
         * @param f starting floating-point value
         * @return The adjacent floating-point value closer to positive
         * infinity.
         * @since 1.6
         */
        public static float NextUp(this float f)
        {
            if (float.IsNaN(f) || f == float.PositiveInfinity)
                return f;
            else
            {
                f += 0.0f;
                return BitConversion.Int32BitsToSingle(BitConversion.SingleToRawInt32Bits(f) +
                                            ((f >= 0.0f) ? +1 : -1));
            }
        }

        /**
         * Returns the floating-point value adjacent to {@code d} in
         * the direction of negative infinity.  This method is
         * semantically equivalent to {@code nextAfter(d,
         * Double.NEGATIVE_INFINITY)}; however, a
         * {@code nextDown} implementation may run faster than its
         * equivalent {@code nextAfter} call.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, the result is NaN.
         *
         * <li> If the argument is negative infinity, the result is
         * negative infinity.
         *
         * <li> If the argument is zero, the result is
         * {@code -Double.MIN_VALUE}
         *
         * </ul>
         *
         * @param d  starting floating-point value
         * @return The adjacent floating-point value closer to negative
         * infinity.
         * @since 1.8
         */
        public static double NextDown(this double d)
        {
            if (double.IsNaN(d) || d == double.NegativeInfinity)
                return d;
            else
            {
                if (d == 0.0)
                    return -double.MinValue;
                else
                    return BitConversion.Int64BitsToDouble(BitConversion.DoubleToRawInt64Bits(d) +
                                                   ((d > 0.0d) ? -1L : +1L));
            }
        }

        /**
         * Returns the floating-point value adjacent to {@code f} in
         * the direction of negative infinity.  This method is
         * semantically equivalent to {@code nextAfter(f,
         * Float.NEGATIVE_INFINITY)}; however, a
         * {@code nextDown} implementation may run faster than its
         * equivalent {@code nextAfter} call.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, the result is NaN.
         *
         * <li> If the argument is negative infinity, the result is
         * negative infinity.
         *
         * <li> If the argument is zero, the result is
         * {@code -Float.MIN_VALUE}
         *
         * </ul>
         *
         * @param f  starting floating-point value
         * @return The adjacent floating-point value closer to negative
         * infinity.
         * @since 1.8
         */
        public static float NextDown(this float f)
        {
            if (float.IsNaN(f) || f == float.NegativeInfinity)
                return f;
            else
            {
                if (f == 0.0f)
                    return -float.MinValue;
                else
                    return BitConversion.Int32BitsToSingle(BitConversion.SingleToRawInt32Bits(f) +
                                                ((f > 0.0f) ? -1 : +1));
            }
        }
    }
}
