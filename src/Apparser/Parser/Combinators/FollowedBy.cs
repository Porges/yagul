using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    public class FollowedBy<TIn, TOut> : Parser<TIn, TOut>, IEquatable<FollowedBy<TIn,TOut>>
    {
        private readonly Parser<TIn, TOut> _previous;
        private readonly Parser<TIn> _next;

        public FollowedBy(Parser<TIn, TOut> previous, Parser<TIn> next)
        {
            _previous = previous;
            _next = next;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return _previous.Run(input)
                     .SelectMany(_ => _next.Run(input));

        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as FollowedBy<TIn, TOut>);
        }

        public override string Name
        {
            get { return "FollowedBy"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _previous.CanMatchWithoutConsumingInput && _next.CanMatchWithoutConsumingInput; }
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            return _previous.RunWithResult(input)
                     .SelectMany(prev => _next.Run(input).Select(_ => prev));
        }

        public bool Equals(FollowedBy<TIn, TOut> other)
        {
            return other != null &&
                   _previous.Equals(other._previous) &&
                   _next.Equals(other._next);
        }
    }
}