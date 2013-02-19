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
                return input.Current;

            return "No value available";
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            if (input.MoveNext())
                return default(Unit);

            return "No value available";
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
    }
}