using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base
{
    public static class Objects
    {
        public static void Swap<T>(ref T left, ref T right)
        {
            var tmp = left;
            left = right;
            right = tmp;
        }

        /// <summary>
        /// Simplifies casting to a base class.
        /// </summary>
        public static T As<T>(this T obj)
        {
            return obj;
        }
    }
}
