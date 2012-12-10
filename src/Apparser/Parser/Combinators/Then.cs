using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Then<TIn> : Parser<TIn>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn> _second;

        public Then(Parser<TIn> first, Parser<TIn> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return _first
                .Run(input)
                .SelectManySuccess(_ => _second.Run(input));
        }
    }

    internal sealed class Then<TIn, TOut> : Parser<TIn, TOut>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn, TOut> _second;

        public Then(Parser<TIn> first, Parser<TIn, TOut> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            return _first
                .Run(input)
                .SelectManySuccess(_ => _second.RunWithResult(input));
        }
    }
}