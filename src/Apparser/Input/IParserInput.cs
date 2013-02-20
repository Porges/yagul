using System;
using System.Collections;
using System.Collections.Generic;

namespace Apparser.Input
{
    public interface IParserInput<out T, TSave> 
        : IEnumerator<T>, IParserInput
        where TSave : IEquatable<TSave>
    {
        TSave Save();
        void Restore(TSave save);
    }

    public interface IParserInput : IEnumerator
    {
    }
}