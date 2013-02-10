using System;

namespace Strung
{
    public struct Count<T> : IEquatable<Count<T>>
    {
        private readonly int _value;

        public Count(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        public static implicit operator Count<T>(int value)
        {
            return new Count<T>(value);
        }

        public bool Equals(Count<T> other)
        {
            return _value == other._value;
        }
    }
}