using System;
using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class Bind<TIn, TMid, TOut> : Parser<TIn, TOut>
    {
        private readonly Func<TMid, Parser<TIn, TOut>> _projection;
        private readonly Parser<TIn, TMid> _parser;

        public Bind(Func<TMid, Parser<TIn, TOut>> projection, Parser<TIn, TMid> parser)
        {
            _projection = projection;
            _parser = parser;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            return _parser
                .RunWithResult(input)
                .SelectManySuccess(x => _projection(x).RunWithResult(input));
        }
    }
}