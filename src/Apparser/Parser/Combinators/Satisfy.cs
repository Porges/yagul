using System;
using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class Satisfy<T> : Parser<T, T>
    {
        private readonly Predicate<T> _predicate;

        public Satisfy(Predicate<T> predicate)
        {
            _predicate = predicate;
        } 

        public override Result<string, T> RunWithResult<TSave>(IParserInput<T,TSave> input)
        {
            if (!input.MoveNext())
                return "Unexpected end of input.";

            if (_predicate(input.Current))
                return input.Current;

            return "Could not satisfy predicate.";
        }
    }
}