using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Porges.Net.Addresses
{
    public struct Ipv6Address : IIpAddress, IEquatable<Ipv6Address>
    {
        private readonly ushort[] _shorts;
        
        public Ipv6Address(Ipv4Address other)
            : this(0,0,0,0,0, 0xFFFF, (ushort)((other.IntegralRepresentation >> 16) & 0xFFFF), (ushort)(other.IntegralRepresentation & 0xFFFF))
        {   
        }

        public static implicit operator Ipv6Address(Ipv4Address other)
        {
            return new Ipv6Address(other);
        }

        private Ipv6Address(ushort[] arr)
        {
            _shorts = arr;
        }

        public Ipv6Address
            (ushort short1, ushort short2, ushort short3, ushort short4, ushort short5, ushort short6,
             ushort short7, ushort short8)
            : this(new []{short1, short2, short3, short4, short5, short6, short7, short8})
        {
        }

        public Ipv6Address(ICollection<ushort> shorts) : this(shorts.ToArray())
        {
            if (shorts.Count != 8)
                throw new ArgumentException("bytes");
        }

        private static void ToBytes(byte[] bytes, int index, ushort s)
        {
            bytes[index*2] = (byte) (0xFF & (s >> 8));
            bytes[index*2+1] = (byte) (0xFF & s);
        }

        public ReadOnlyCollection<ushort> ToShorts()
        {
            return new ReadOnlyCollection<ushort>(_shorts);
        }

        public byte[] ToBytes()
        {
            var result = new byte[16];
            var shorts = ToShorts();
            for (int i = 0; i < 8; ++i)
                ToBytes(result, i, shorts[i]);
            return result;
        }

        public bool Equals(Ipv6Address other)
        {
            unsafe
            {
                fixed (ushort* thisPtr = _shorts)
                fixed (ushort* otherPtr = other._shorts)
                {
                    var thisLongPtr = (IntPtr*) thisPtr;
                    var otherLongPtr = (IntPtr*) otherPtr;

                    for (int i = 0; i < 8*sizeof(ushort)/IntPtr.Size; ++i)
                    {
                        if (thisLongPtr[i] != otherLongPtr[i])
                            return false;
                    }
                    return true;
                }
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Ipv6Address?;
            return other.HasValue && Equals(other.Value);
        }

        public override int GetHashCode()
        {
            // TODO: better
            return _shorts[0] << 16 | _shorts[7];
        }

        public static bool TryParse(string input, out Ipv6Address address)
        {
            address = default(Ipv6Address);

            if (string.IsNullOrEmpty(input))
                return false;

            var shorts = new ushort[8];
            var index = 0;
            var count = 0;
            var skippedFromIndex = 8;
            var skipped = false;

            if (SimpleParser.ReadExact(input, ref index, ':'))
            {
                if (!SimpleParser.ReadExact(input, ref index, ':'))
                    return false;

                skipped = true;
                skippedFromIndex = count;
            }

            do
            {
                if (!SimpleParser.ReadHexShort(input, ref index, out shorts[count]))
                    break;
                
                if (++count == 8)
                    break;

                if (!SimpleParser.ReadExact(input, ref index, ':'))
                {
                    break;
                }

                if (SimpleParser.Eof(input, index))
                    return false;

                if (SimpleParser.ReadExact(input, ref index, ':'))
                {
                    if (skipped)
                        return false;

                    skipped = true;
                    skippedFromIndex = count;
                }
            } while (true);

            if (!SimpleParser.Eof(input, index))
                return false;

            var moveCount = count - skippedFromIndex;
            var moveDistance = shorts.Length - count;
            Array.Copy(shorts, skippedFromIndex, shorts, skippedFromIndex + moveDistance, moveCount);
            Array.Clear(shorts, skippedFromIndex, moveDistance);

            address = new Ipv6Address(shorts);
            return true;
        }

        public static Ipv6Address Unspecified
        {
            get { return default(Ipv6Address); }
        }

        public bool IsUnspecified
        {
            get { return Equals(Unspecified); }
        }

        public override string ToString()
        {
            var longestCount = 0;
            var currentCount = 0;
            var startsAt = -1;
            for (int i = 0; i < _shorts.Length; ++i)
            {
                if (_shorts[i] == 0)
                {
                    ++currentCount;
                }
                else
                {
                    if (currentCount > longestCount)
                    {
                        longestCount = currentCount;
                        startsAt = i-currentCount;
                    }
                    currentCount = 0;
                }
            }

            if (currentCount > longestCount)
            {
                longestCount = currentCount;
                startsAt = 8-currentCount;
            }

            var result = new StringBuilder();
            bool skipColon = true;
            for (int i = 0; i < _shorts.Length; )
            {
                if (startsAt == i)
                {
                    result.Append("::");
                    i += longestCount;
                    skipColon = true;
                }
                else
                {
                    if (!skipColon)
                    {
                        result.Append(':');
                    }
                    result.Append(_shorts[i].ToString("x"));
                    i++;
                    skipColon = false;
                }
            }

            return result.ToString();
        }

        public static bool operator ==(Ipv6Address left, Ipv6Address right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ipv6Address left, Ipv6Address right)
        {
            return !(left == right);
        }
    }
}
