using System.Collections;
using System.IO;
using Apparser.Parser;

namespace Apparser.Input
{
    public class StreamParser : IParserInput<byte, StreamParser.StreamParserState>
    {
        public struct StreamParserState
        {
            internal readonly long Position;

            internal StreamParserState(long position)
            {
                Position = position;
            }
        }

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

        public StreamParserState Save()
        {
            return new StreamParserState(_stream.Position);
        }

        public void Restore(StreamParserState save)
        {
            _stream.Position = save.Position;
        }
    }
}