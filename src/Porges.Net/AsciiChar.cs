namespace Porges.Net
{
    public static class AsciiChar
    {
        public static bool IsAsciiDigit(this char c)
        {
            return c >= '0' && c <= '9';
        }

        public static int AsciiDigitValue(this char c)
        {
            return c - '0';
        }

        public static char QuickToUpper(this char c)
        {
            return (char)(c & ~(1 << 5));
        }

        public static int HexValue(this char c)
        {
            var c2 = c.QuickToUpper();
            if (c2 >= 'A') return c2 - 'A' + 10;

            return c - '0';
        }

        public static bool IsAsciiAlphaNum(this char c)
        {
            return c.IsAsciiLetter() || c.IsAsciiDigit();
        }

        public static bool IsAsciiLetter(this char c)
        {
            return c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z';
        }

        public static bool IsAsciiHex(this char c)
        {
            return c.IsAsciiDigit() || c >= 'A' && c <= 'F' || c >= 'a' && c <= 'f';
        }
    }
}