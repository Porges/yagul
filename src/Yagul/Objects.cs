using JetBrains.Annotations;

namespace Yagul
{
    /// <summary>
    /// Methods so generic that they don't belong anywhere else.
    /// </summary>
    public static class Objects
    {
        /// <summary>
        /// Swaps the two objects.
        /// </summary>
        /// <typeparam name="T">The type of the objects.</typeparam>
        [PublicAPI]
        public static void Swap<T>(ref T left, ref T right)
        {
            var tmp = left;
            left = right;
            right = tmp;
        }
    }

    public static class Arrays<T>
    {
        public static readonly T[] Empty = new T[0]; 
    }
}
