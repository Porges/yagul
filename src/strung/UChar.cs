using System;

namespace Strung
{
    /// <summary>
    /// Represents a Unicode codepoint - an integer in the range 0-0x10FFFF.
    /// </summary>
    public struct UChar : IEquatable<UChar>, IComparable<UChar>
    {
        private readonly int _value;

        public UChar(int value)
        {
            if (value < MinValue || value > MaxValue) throw new ArgumentOutOfRangeException("value");

            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        public const int MinValue = 0;
        public const int MaxValue = 0x10FFFF;

        public const int HighSurrogateStart = 0xD800;
        public const int LowSurrogateEnd = 0xDFFF;

        public bool Equals(UChar other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            var other = obj as UChar?;
            return other.HasValue && Equals(other.Value);
        }
        
        public bool IsSurrogate
        {
            get { return _value >= HighSurrogateStart && _value <= LowSurrogateEnd; }
        }

        public bool IsScalarValue
        {
            get { return !IsSurrogate; }
        }

        public override int GetHashCode()
        {
            return _value;
        }
        
        public static implicit operator UChar(char value)
        {
            return new UChar(value);
        }

        public static bool operator ==(UChar left, UChar right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UChar left, UChar right)
        {
            return !(left == right);
        }

        public int CompareTo(UChar other)
        {
            return _value.CompareTo(other._value);
        }

        public override string ToString()
        {
            return char.ConvertFromUtf32(_value);
        }
    }
}