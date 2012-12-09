using System;

namespace Strung
{
    public struct UChar
    {
        private readonly int _value;

        public UChar(int value)
        {
            if (value < 0 || value > 0x10FFFF) throw new ArgumentOutOfRangeException("value");
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }
    }
}