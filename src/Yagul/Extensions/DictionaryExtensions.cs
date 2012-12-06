using System.Collections.Generic;
using JetBrains.Annotations;

namespace Porges.Absent.Extensions
{
    /// <summary>
    /// Extension methods based upon the <see cref="IDictionary{TKey,TValue}"/> class.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Returns the key's matching value from the dictionary, or a default value if the key doesn't exist.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="this">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">A default value to return if the key is not present in the dictionary.</param>
        /// <returns>Either the matching value for the key, or the default value.</returns>
        [PublicAPI]
        public static TValue GetValueOrDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, TValue defaultValue = default(TValue))
        {
            TValue result;
            if (!@this.TryGetValue(key, out result))
                result = defaultValue;

            return result;
        }

        /// <summary>
        /// Returns the key's matching value from the dictionary, or creates and returns a default value if the key doesn't exist.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="this">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">A default value to return if the key is not present in the dictionary.</param>
        /// <returns>Either the matching value for the key, or the default value.</returns>
        [PublicAPI]
        public static TValue GetValueOrCreate<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, TValue defaultValue = default(TValue))
        {
            TValue result;
            if (!@this.TryGetValue(key, out result))
            {
                @this[key] = result = defaultValue;
            }

            return result;
        }

    }
}
