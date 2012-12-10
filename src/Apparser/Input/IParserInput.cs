using System.Collections.Generic;

namespace Apparser.Input
{
    public interface IParserInput<out T, TSave> : IEnumerator<T>
    {
        TSave Save();
        void Restore(TSave save);
    }
}