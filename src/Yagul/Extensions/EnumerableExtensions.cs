using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Yagul.Extensions
{
    public static class EnumerableExtensions
    {
        public static void CopyTo<T>(this IEnumerable<T> source, IObserver<T> observer)
        {
            foreach (var value in source)
            {
                observer.OnNext(value);
            }

            observer.OnCompleted();
        }
    }
}
