using System;
using System.Collections;

namespace Apparser.Input
{
    public class StringParser : IParserInput<char, StringParser.State>
    {
        public struct State : IEquatable<State>
        {
            internal readonly int Index;

            internal State(int index)
            {
                Index = index;
            }

            public override bool Equals(object obj)
            {
                var state = obj as State?;
                return state.HasValue && Equals(state.Value);
            }

            public override int GetHashCode()
            {
                return Index;
            }

            public bool Equals(State other)
            {
                return Index == other.Index;
            }
        }

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

        public State Save()
        {
            return new State(_position);
        }

        public void Restore(State save)
        {
            _position = save.Index;
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