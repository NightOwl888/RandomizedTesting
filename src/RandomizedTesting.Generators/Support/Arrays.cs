using System.Runtime.CompilerServices;

namespace RandomizedTesting.Generators
{
    internal static class Arrays
    {
        /// <summary>
        /// Returns an empty array.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <returns>An empty array.</returns>
        // Since Array.Empty<T>() doesn't exist in all supported platforms, we
        // have this wrapper method to add support.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Empty<T>()
        {
#if FEATURE_ARRAYEMPTY
            return Array.Empty<T>();
#else
            return EmptyArrayHolder<T>.Empty;
#endif
        }

        private static class EmptyArrayHolder<T>
        {
#pragma warning disable CA1825 // Avoid zero-length array allocations.
            public static readonly T[] Empty = new T[0];
#pragma warning restore CA1825 // Avoid zero-length array allocations.
        }
    }
}
