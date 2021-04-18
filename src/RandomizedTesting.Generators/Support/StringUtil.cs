using J2N;
using System;

namespace RandomizedTesting.Generators
{
    internal static class StringUtil
    {
        // TODO: Move to J2N? We don't have a way to convert codepoints to a string easily.
        // In Java the String class has a constructor overload for this.
        public static string CodePointsToString(int[] codePoints)
        {
            if (codePoints is null)
                throw new ArgumentNullException(nameof(codePoints));

            return CodePointsToString(codePoints, 0, codePoints.Length);
        }

        public static string CodePointsToString(int[] codePoints, int startIndex, int length)
        {
            if (codePoints is null)
                throw new ArgumentNullException(nameof(codePoints));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex > codePoints.Length - length)
                throw new ArgumentOutOfRangeException(nameof(length), /*SR2.ArgumentOutOfRange_IndexLength*/"Index and length must refer to a location within the array.");

            int countThreashold = 1024; // If the number of chars exceeds this, we count them instead of allocating count * 2
            // as a first approximation, assume each codepoint 
            // is 2 characters (since it cannot be longer than this)
            int arrayLength = length * 2;
            // if we go over the threashold, count the number of 
            // chars we will need so we can allocate the precise amount of memory
            if (length > countThreashold)
            {
                arrayLength = 0;
                for (int r = startIndex, e = startIndex + length; r < e; ++r)
                {
                    arrayLength += Character.CharCount(codePoints[r]);
                }
                if (arrayLength < 1)
                {
                    arrayLength = length * 2;
                }
            }
            // Initialize our array to our exact or oversized length.
            // It is now safe to assume we have enough space for all of the characters.
            char[] buffer = new char[arrayLength];
            int totalLength = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                totalLength += Character.ToChars(codePoints[i], buffer, totalLength);
            }
            return new string(buffer, 0, totalLength);
        }
    }
}
