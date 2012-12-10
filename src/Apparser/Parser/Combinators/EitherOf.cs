using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class EitherOf<TIn, TOut> : Parser<TIn, TOut>
    {
        private readonly Parser<TIn, TOut> _first;
        private readonly Parser<TIn, TOut> _second;

        public EitherOf(Parser<TIn, TOut> first, Parser<TIn, TOut> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            return _first
                .RunWithResult(input)
                .SelectManyFailure(_ =>
                {
                    input.Restore(saved);
                    return _second.RunWithResult(input);
                });
        }
    }

    internal sealed class EitherOf<TIn> : Parser<TIn>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn> _second;

        public EitherOf(Parser<TIn> first, Parser<TIn> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            return _first
                .Run(input)
                .SelectManyFailure(_ =>
                {
                    input.Restore(saved);
                    return _second.Run(input);
                });
        }
    }
}