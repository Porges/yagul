using System;
using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class Exactly<TIn> : Parser<TIn, TIn>, IEquatable<Exactly<TIn>>
    {
        private readonly TIn _item;

        public Exactly(TIn item)
        {
            _item = item;
        }

        public override Result<string, TIn> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            if (!input.MoveNext())
                return string.Format("Unexpected end of input.");

            if (input.Current.Equals(_item))
                return _item;

            return string.Format("Expected '{0}', got '{1}'.", _item, input.Current);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Exactly<TIn>);
        }

        public bool Equals(Exactly<TIn> other)
        {
            return other != null
                   && Equals(_item, other._item);
        }
    }
}