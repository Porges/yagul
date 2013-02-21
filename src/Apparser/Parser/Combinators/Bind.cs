using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Bind<TIn, TMid, TOut> : Parser<TIn, TOut>, IEquatable<Bind<TIn, TMid, TOut>>
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


        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return _parser.RunWithResult(input).SelectManySuccess(x => _projection(x).Run(input));
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Bind<TIn, TMid, TOut>);
        }

        public bool Equals(Bind<TIn, TMid, TOut> other)
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