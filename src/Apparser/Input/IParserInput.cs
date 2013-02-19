using System;
using System.Collections.Generic;

namespace Apparser.Input
{
    public interface IParserInput<out T, TSave> 
        : IEnumerator<T>
        where TSave : IEquatable<TSave>
    {
        TSave Save();
        void Restore(TSave save);
    }
}