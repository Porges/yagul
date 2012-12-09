using System.Collections;
using System.IO;
using Apparser;

namespace ConsoleApplication1
{
    class StreamParser : IParserInput<byte, long>
    {
        private readonly Stream _stream;
        private byte _current;

        public StreamParser(Stream stream)
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public bool MoveNext()
        {
            var b = _stream.ReadByte();
            if (b == -1)
                return false;
            _current = (byte)b;
            return true;
        }

        public void Reset()
        {
            _stream.Seek(0, SeekOrigin.Begin);
        }

        public byte Current { get { return _current; } }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public long Save()
        {
            return _stream.Position;
        }

        public void Restore(long save)
        {
            _stream.Position = save;
        }
    }

    class StringParser : IParserInput<char, int>
    {
        private int _position = -1;
        private readonly string _input;
        public StringParser(string input)
        {
            _input = input;
        }

        public StringParser(StringParser parser)
        {
            _input = parser._input;
            _position = parser._position;
        }

        public int Save()
        {
            return _position;
        }

        public void Restore(int save)
        {
            _position = save;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return ++_position < _input.Length;
        }

        public void Reset()
        {
            _position = 0;
        }

        public char Current { get { return _input[_position]; } }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}