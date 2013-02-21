using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

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
                return new Failure<string, TIn>(string.Format("Unexpected end of input."));

            if (input.Current.Equals(_item))
                return new Outcomes.Success<string, TIn>(_item);

            return new Failure<string, TIn>(string.Format("Expected '{0}', got '{1}'.", _item, input.Current));
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            if (!input.MoveNext())
                return new Failure<string, Unit>(string.Format("Unexpected end of input."));

            if (input.Current.Equals(_item))
                return new Outcomes.Success<string, Unit>(default(Unit));

            return new Failure<string, Unit>(string.Format("Expected '{0}', got '{1}'.", _item, input.Current));
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
        public override string Name
        {
            get { return "'" + Sanitise(_item.ToString()) + "'"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return false; }
        }

        private string Sanitise(string input)
        {
            return input.Replace("\r", "CR").Replace("\n", "LF");
        }
    }
}