using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Yagul.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Concatenates two arrays together to form a new one.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="left">The left array.</param>
        /// <param name="right">The right array.</param>
        /// <param name="shortCutAllowed">Is the function allowed to return one of the input arrays if the other is empty?</param>
        /// <returns>The two arrays concatenated into one.</returns>
        [Pure]
        [PublicAPI]
        public static T[] Concat<T>(this T[] left, T[] right, bool shortCutAllowed = false)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            if (shortCutAllowed)
            {
                if (left.Length == 0) return right;
                if (right.Length == 0) return left;
            }

            var result = new T[left.Length + right.Length];
            Array.Copy(left, 0, result, 0, left.Length);
            Array.Copy(right, 0, result, left.Length, right.Length);
            return result;
        }

        /// <summary>
        /// Concatenates two arrays together to form a new one.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="left">The left array.</param>
        /// <param name="right">The right array.</param>
        /// <param name="shortCutAllowed">Is the function allowed to return one of the input arrays if the other is null?</param>
        /// <returns>The two arrays concatenated into one.</returns>
        [Pure]
        [PublicAPI]
        public static T[] ConcatAllowNulls<T>(this T[] left, T[] right, bool shortCutAllowed = false)
        {
            if (left == null || left.Length == 0)
            {
                if (shortCutAllowed) return right;
                if (right == null) return null;
                return (T[]) right.Clone();
            }

            if (right == null || right.Length == 0)
            {
                if (shortCutAllowed) return left;
                return (T[]) left.Clone();
            }

            var result = new T[left.Length + right.Length];
            Array.Copy(left, 0, result, 0, left.Length);
            Array.Copy(right, 0, result, left.Length, right.Length);
            return result;
        }

        // this is a giant hack
        private static readonly FieldInfo _listArray = typeof(List<byte>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
        private static byte[] GetListBackingArray(List<byte> list)
        {
            return (byte[])_listArray.GetValue(list);
        }

        [PublicAPI]
        public static bool EqualBytes(this List<byte> left, List<byte> right)
        {
            if (left == right)
                return true;

            if (left == null)
                return false;

            if (left.Count != right.Count)
                return false;

            return EqualBytesInner(left, right, left.Count);
        }

        private static bool EqualBytesInner(this List<byte> left, List<byte> right, int count)
        {
            Debug.Assert(left != null);
            Debug.Assert(right != null);
            Debug.Assert(count >= 0);
            Debug.Assert(count <= left.Count);
            Debug.Assert(count <= right.Count);

            unsafe
            {
                fixed (byte* bytes = GetListBackingArray(left))
                fixed (byte* otherBytes = GetListBackingArray(right))
                {
                    var longLength = count / sizeof(IntPtr);
                    var longs = (IntPtr*)bytes;
                    var otherLongs = (IntPtr*)otherBytes;

                    for (int i = 0; i < longLength; ++i)
                    {
                        if (longs[i] != otherLongs[i])
                            return false;
                    }

                    // do leftovers at the end of the string
                    for (int i = longLength * sizeof(IntPtr); i < count; ++i)
                    {
                        if (bytes[i] != otherBytes[i])
                            return false;
                    }
                }
            }

            return true;
        }

        [PublicAPI]
        public static bool EqualBytes(this List<byte> left, int leftCount, List<byte> right, int rightCount)
        {
            if (left == right)
                return true;

            if (left == null)
                return false;

            if (leftCount != rightCount)
                return false;

            return EqualBytesInner(left, right, leftCount);
        }
    }
}
