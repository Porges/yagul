using System;
using System.Collections;
using System.IO;

namespace Apparser.Input
{
    public class StreamParser : IParserInput<byte, StreamParser.State>
    {
        public struct State : IEquatable<State>
        {
            internal readonly long Position;

            internal State(long position)
            {
                Position = position;
            }

            public override bool Equals(object obj)
            {
                var state = obj as State?;
                return state.HasValue && Equals(state.Value);
            }

            public override int GetHashCode()
            {
                return Position.GetHashCode();
            }

            public bool Equals(State other)
            {
                return Position == other.Position;
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

        public State Save()
        {
            return new State(_stream.Position);
        }

        public void Restore(State save)
        {
            _stream.Position = save.Position;
        }
    }
}