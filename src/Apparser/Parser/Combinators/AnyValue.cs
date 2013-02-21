using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class AnyValue<TIn> : Parser<TIn,TIn>, IEquatable<AnyValue<TIn>>
    {
        private AnyValue()
        {}

        public override Result<string, TIn> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            if (input.MoveNext())
                return new Outcomes.Success<string, TIn>(input.Current);

            return new Failure<string, TIn>("No value available");
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            if (input.MoveNext())
                return new Outcomes.Success<string, Unit>(default(Unit));

            return new Failure<string, Unit>("No value available");
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as AnyValue<TIn>);
        }

        static readonly AnyValue<TIn> Instance = new AnyValue<TIn>();
        public static Parser<TIn, TIn> Create()
        {
            return Instance;
        }

        public bool Equals(AnyValue<TIn> other)
        {
            return other != null;
		}
		
        public override string Name
        {
            get { return "anything"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return false; }
        }
    }
}