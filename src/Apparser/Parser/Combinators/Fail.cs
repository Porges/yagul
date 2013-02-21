using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Fail<T> : Parser<T>, IEquatable<Fail<T>>
    {
        private readonly string _message;

        public Fail(string message)
        {
            _message = message;
        }
        
        public string Message
        {
            get { return _message; }
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<T,TSave> input)
        {
            return new Failure<string, Unit>(_message);
        }

        public override bool Equals(Parser<T> other)
        {
            return Equals(other as Fail<T>);
        }

        public bool Equals(Fail<T> other)
        {
            return other != null;
        }
        public override string Name
        {
            get { return "failure"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return false; }
        }
    }
}