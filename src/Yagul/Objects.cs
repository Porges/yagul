using JetBrains.Annotations;

namespace Porges.Absent
{
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
}
