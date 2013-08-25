using System.Collections;
using Apparser.Input;

namespace ConsoleApplication1
{
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