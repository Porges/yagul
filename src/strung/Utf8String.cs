using System.Collections.Generic;
using System.Text;

namespace Strung
{
    public class UTF8String : EncodedString<UTF8Encoding>, IUnicodeString
    {
        public UTF8String(string value) : base(value)
        { }

        private UTF8String(ICollection<byte> value) : base(value)
        {
        }

        public static implicit operator UTF8String(ASCIIString other)
        {
            return other == null ? null : new UTF8String(other._bytes);
        }
    }

    public class ASCIIString : EncodedString<ASCIIEncoding>, IUnicodeString
    {
        public ASCIIString(string value) : base(value)
        { }

        public override UChar this[Ix<UChar> index]
        {
            get
            {
                return new UChar(base[new Ix<byte>(index.Value)]);
            }
        }
    }
}
