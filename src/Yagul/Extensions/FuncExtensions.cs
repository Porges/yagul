using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yagul.Extensions
{
    public static class FuncExtensions
    {
        public static Func<T1, Func<T2, TOut>> Curry<T1, T2, TOut>(this Func<T1, T2, TOut> func)
        {
            return arg1 => arg2 => func(arg1, arg2);
        }

        public static Func<T1, Func<T2, Func<T3, TOut>>> Curry<T1, T2, T3, TOut>(this Func<T1, T2, T3, TOut> func)
        {
            return arg1 => arg2 => arg3 => func(arg1, arg2, arg3);
        }

        public static Func<T1, Func<T2, Func<T3, Func<T4, TOut>>>> Curry<T1, T2, T3, T4, TOut>(this Func<T1, T2, T3, T4, TOut> func)
        {
            return arg1 => arg2 => arg3 => arg4 => func(arg1, arg2, arg3, arg4);
        }
    }
}
