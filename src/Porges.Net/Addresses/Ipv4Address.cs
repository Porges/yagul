using System;
using System.Collections.Generic;
using Apparser.Input;
using Apparser.Parser;

namespace Porges.Net.Addresses
{
    public struct Ipv4Address : IIpAddress, IEquatable<Ipv4Address>, IComparable<Ipv4Address>
    {
        private readonly ulong _ip;

        public Ipv4Address(ulong ip)
            : this()
        {
            _ip = ip;
        }

        public Ipv4Address(byte b1, byte b2, byte b3, byte b4)
            : this((ulong)b1 << 24 | (ulong)b2 << 16 | (ulong)b3 << 8 | b4)
        {}

        public Ipv4Address(IList<byte> bytes) : this (bytes[0], bytes[1], bytes[2], bytes[3])
        {
            if (bytes.Count != 4)
                throw new ArgumentException("bytes");
        }

        public ulong IntegralRepresentation
        {
            get { return _ip; }
        }

        public byte[] ToBytes()
        {
            return new []
                       {
                           (byte)(0xFF & (_ip >> 24)),
                           (byte)(0xFF & (_ip >> 16)),
                           (byte)(0xFF & (_ip >> 8)),
                           (byte)(0xFF & (_ip))
                       };
        }

        public bool Equals(Ipv4Address other)
        {
            return _ip == other._ip;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Ipv4Address?;
            return other.HasValue && Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return (int)_ip;
        }

        public static bool TryParse(string input, out Ipv4Address address)
        {
            var result = _parser.RunWithResult(new StringParser(input));
            return result.TryGetSuccess(out address);
        }

        private static readonly Parser<char, Ipv4Address> _parser = MakeParser();
        
        static Parser<char, Ipv4Address> MakeParser()
        {
            var group = Parser.Satisfy((char c) => c >= '0' && c <= '9')
                            .Many(1, 3, () => 0,
                            (result, newDigit) =>
                            {
                                result *= 10;
                                result += newDigit.AsciiDigitValue();
                                return result;
                            }).SelectMany(i =>
                            {
                                if (i > byte.MaxValue) return Parser.Fail<char, byte>("Invalid byte value: " + i);
                                return Parser.Return<char, byte>((byte) i);
                            });
            
            var dot = Parser.Exactly('.');
            var ipAddress = group.SepBy(dot, 4, 4)
                                 .Select(bytes => new Ipv4Address(bytes[0], bytes[1], bytes[2], bytes[3]))
                                 .FollowedBy(Parser.EndOfInput<char>());
            return ipAddress;
        }
        
        public int CompareTo(Ipv4Address other)
        {
            return _ip.CompareTo(other._ip);
        }

        public override string ToString()
        {
            var bytes = ToBytes();
            return string.Format("{0}.{1}.{2}.{3}", bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public static bool operator ==(Ipv4Address left, Ipv4Address right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ipv4Address left, Ipv4Address right)
        {
            return !(left == right);
        }
    }
}