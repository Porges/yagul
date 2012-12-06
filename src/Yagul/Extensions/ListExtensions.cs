using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Porges.Absent.Extensions
{
    /// <summary>
    /// Extension methods based upon the <see cref="IList{T}"/> interface.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Selects a random value from the list.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="this">The list.</param>
        /// <param name="random">The <see cref="Random"/> instance to use.</param>
        /// <returns>A random value from the list provided.</returns>
        [PublicAPI]
        public static T SelectRandom<T>([NotNull] this IList<T> @this, [NotNull] Random random)
        {
            return @this[random.Next(@this.Count)];
        }

        /// <summary>
        /// Swaps two items in a list.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="this">The list.</param>
        /// <param name="i">One index.</param>
        /// <param name="j">The other index.</param>
        [PublicAPI]
        public static void Swap<T>([NotNull] this IList<T> @this, int i, int j)
        {
            var tmp = @this[i];
            @this[i] = @this[j];
            @this[j] = tmp;
        }

        /// <summary>
        /// Shuffles a list in-place.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="this">The list.</param>
        /// <param name="random">The <see cref="Random"/> instance to use.</param>
        [PublicAPI]
        public static void Shuffle<T>([NotNull] this IList<T> @this, [NotNull] Random random)
        {
            for (var i = @this.Count - 1; i >= 1; --i)
            {
                @this.Swap(i, random.Next(i + 1));
            }
        }
    }
}
