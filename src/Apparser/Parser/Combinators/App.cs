using System;
using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class App<TIn, TMid, TOut> : Parser<TIn, TOut>, IEquatable<App<TIn, TMid, TOut>>
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

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as App<TIn, TMid, TOut>);
        }

        public bool Equals(App<TIn, TMid, TOut> other)
        {
            return other != null &&
                   Equals(_parser, other._parser) &&
                   Equals(_projection, other._projection);
        }

        public override string Name
        {
            get { return _parser.Name; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _parser.CanMatchWithoutConsumingInput; }
        }
    }
}