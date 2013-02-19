using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class EOFValue<TIn> : Parser<TIn>, IEquatable<EOFValue<TIn>>
    {
        private static readonly EOFValue<TIn> _instance = new EOFValue<TIn>();

        private EOFValue() { }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn,TSave> input)
        {
            if (input.MoveNext())
                return "Expected end of input.";

            return default(Unit);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as EOFValue<TIn>);
        }

        public static EOFValue<TIn> Instance
        {
            get { return _instance; }
        }

        public bool Equals(EOFValue<TIn> other)
        {
            return other != null;
        }
    }
}