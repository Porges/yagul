using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yagul
{
    public class CollectionAddingObserver<T> : IObserver<T>
    {
        private readonly ICollection<T> _collection;

        public CollectionAddingObserver(ICollection<T> collection)
        {
            _collection = collection;
        }

        public void OnNext(T value)
        {
            _collection.Add(value);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }
}
