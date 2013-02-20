using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Yagul.Extensions;

namespace Canasta
{
    /// <summary>
    /// A very simple persistent list that can be extended and previous elements will be shared.
    /// </summary>
    public class ExtendablePersistentList<T> : IReadOnlyList<T>, ICollection<T>
        where T : struct // allows us to return indexed items without them being modified...
    {
        internal readonly List<T> _contents;

        [Pure]
        public int Count { get; private set; }

        [Pure]
        public ExtendablePersistentList(): this(new List<T>())
        { }

        [Pure]
        public ExtendablePersistentList(int capacity) : this (new List<T>(capacity))
        { }

        [Pure]
        private ExtendablePersistentList(List<T> list) : this(list.Count, list)
        { }

        [Pure]
        private ExtendablePersistentList(int count, List<T> contents)
        {
            Count = count;
            _contents = contents;
        }

        [Pure]
        private bool OwnsList
        {
            get { return Count == _contents.Count; }
        }

        [Pure]
        public IEnumerator<T> GetEnumerator()
        {
            return _contents.Take(Count).GetEnumerator();
        }

        [Pure]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Pure]
        public ExtendablePersistentList<T> Add(T item)
        {
            if (OwnsList)
            {
                _contents.Add(item);
                return new ExtendablePersistentList<T>(_contents);
            }
            else
            {
                var newList = new List<T>(_contents);
                newList.Add(item);
                return new ExtendablePersistentList<T>(newList);
            }
        }
        
        [Pure]
        public static ExtendablePersistentList<T> operator +(ExtendablePersistentList<T> left, IEnumerable<T> right)
        {
            if (left.OwnsList)
            {
                left._contents.AddRange(right);
                return new ExtendablePersistentList<T>(left._contents);
            }
            else
            {
                var rightCollection = right as ICollection<T>;
                var rightCount = rightCollection != null ? rightCollection.Count : 0;

                var newList = new List<T>(left._contents.Count + rightCount);
                newList.AddRange(left);
                newList.AddRange(right);
                return new ExtendablePersistentList<T>(newList);
            }
        }
            
        [Pure]
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                    throw new ArgumentOutOfRangeException("index");

                return _contents[index];
            }
        }

        [Pure]
        public bool IsReadOnly { get { return true; } }

        [Pure]
        public bool Contains(T item)
        {
            return _contents.Take(Count).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in this)
            {
                array[arrayIndex++] = item;
            }
        }
        
        [Pure]
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

    }

    public static class PersistentListExtensions
    {
        public static bool EqualBytes(this ExtendablePersistentList<byte> left, ExtendablePersistentList<byte> right)
        {
            return ArrayExtensions.EqualBytes(left._contents, left.Count, right._contents, right.Count);
        }
    }
}
