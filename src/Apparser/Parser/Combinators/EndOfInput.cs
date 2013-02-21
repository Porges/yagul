using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class EndOfInput<TIn> : Parser<TIn>, IEquatable<EndOfInput<TIn>>
    {
        private static readonly EndOfInput<TIn> _instance = new EndOfInput<TIn>();

        private EndOfInput() { }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn,TSave> input)
        {
            if (input.MoveNext())
                return new Failure<string, Unit>("Expected end of input.");

            return new Outcomes.Success<string, Unit>(default(Unit));
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as EndOfInput<TIn>);
        }

        public static EndOfInput<TIn> Instance
        {
            get { return _instance; }
        }

        public bool Equals(EndOfInput<TIn> other)
        {
            return other != null;
        }
        public override string Name
        {
            get { return "end of file"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return true; }
        }
    }
}