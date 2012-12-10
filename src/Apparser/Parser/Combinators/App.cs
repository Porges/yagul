using System;
using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class App<TIn, TMid, TOut> : Parser<TIn, TOut>
    {
        private readonly Parser<TIn, TMid> _parser;
        private readonly Func<TMid, TOut> _projection;

        public App(Func<TMid, TOut> projection, Parser<TIn, TMid> parser)
        {
            _projection = projection;
            _parser = parser;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            return _parser.RunWithResult(input).SelectSuccess(_projection);
        }
    }
}