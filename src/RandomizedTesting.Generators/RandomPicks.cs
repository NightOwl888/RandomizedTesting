using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomizedTesting.Generators
{
    /// <summary>
    /// Random selections of objects.
    /// </summary>
    public class RandomPicks
    {
        /// <summary>
        /// Pick a random element from the <paramref name="list"/> (which may be an array).
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="list"/> contains no items.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> or <paramref name="list"/> is <c>null</c>.</exception>
        public static T RandomFrom<T>(Random random, IList<T> list)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count == 0)
            {
                throw new ArgumentException("Can't pick a random object from an empty list.");
            }
            return list[random.Next(0, list.Count)];
        }

        /// <summary>
        /// Pick a random element from the <paramref name="collection"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains no items.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> or <paramref name="collection"/> is <c>null</c>.</exception>
        public static T RandomFrom<T>(Random random, ICollection<T> collection)
        {
            if (random is null)
                throw new ArgumentNullException(nameof(random));
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (collection.Count == 0)
            {
                throw new ArgumentException("Can't pick a random object from an empty collection.");
            }
            return collection.ElementAt(random.Next(0, collection.Count));
        }
    }
}
