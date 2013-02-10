using System;

namespace Strung
{
    public struct Ix<T> : IEquatable<Ix<T>>
    {
        private readonly int _value;

        public Ix(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        public static Count<T> operator -(Ix<T> left, Ix<T> right)
        {
            return new Count<T>(left.Value - right.Value);
        }

        public static Ix<T> operator +(Ix<T> left, Count<T> right)
        {
            return new Ix<T>(left.Value + right.Value);
        }

        public static Ix<T> operator +(Count<T> left, Ix<T> right)
        {
            return new Ix<T>(left.Value + right.Value);
        }

        public static implicit operator Ix<T>(int value)
        {
            return new Ix<T>(value);
        }

        public bool Equals(Ix<T> other)
        {
            return _value == other._value;
        }
    }
}