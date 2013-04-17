namespace Porges.Net
{
    public static class SimpleParser
    {
        public static bool ReadByte(string input, ref int index, out byte value)
        {
            int digitValue;
            if (!ReadDigit(input, ref index, out digitValue))
            {
                value = 0;
                return false;
            }

            int result = digitValue;
            for (int count = 0; count < 2; ++count)
            {
                if (!ReadDigit(input, ref index, out digitValue))
                {
                    break;
                }

                result *= 10;
                result += digitValue;
            }

            if (result > byte.MaxValue)
            {
                value = 0;
                return false;
            }

            value = (byte)result;
            return true;
        }

        public static bool ReadDigit(string input, ref int index, out int value)
        {
            if (Eof(input, index))
            {
                value = 0;
                return false;
            }

            var c = input[index];
            if (c.IsAsciiDigit())
            {
                index++;
                value = c.AsciiDigitValue();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool ReadHexShort(string input, ref int index, out ushort value)
        {
            int digitValue;
            if (!ReadHexDigit(input, ref index, out digitValue))
            {
                value = 0;
                return false;
            }

            int result = digitValue;
            for (int count = 0; count < 4; ++count)
            {
                if (!ReadHexDigit(input, ref index, out digitValue))
                {
                    break;
                }

                result *= 16;
                result += digitValue;
            }

            if (result > ushort.MaxValue)
            {
                value = 0;
                return false;
            }

            value = (ushort)result;
            return true;
        }

        public static bool ReadHexDigit(string input, ref int index, out int value)
        {
            if (Eof(input, index))
            {
                value = 0;
                return false;
            }

            var c = input[index];
            if (c.IsAsciiHex())
            {
                index++;
                value = c.HexValue();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static bool ReadExact(string input, ref int index, char value)
        {
            if (index >= input.Length)
                return false;

            if (input[index] != value)
                return false;

            index++;
            return true;
        }

        public static bool Eof(string input, int index)
        {
            return index == input.Length;
        }
    }
}