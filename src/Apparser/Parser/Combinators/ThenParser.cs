using System;
using System.Linq;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class ThenParser<TIn> : Parser<TIn>, IEquatable<ThenParser<TIn>>
    {
        private readonly Parser<TIn>[] _parsers;

        private ThenParser(Parser<TIn>[] parsers)
        {
            _parsers = parsers;
        }

        private ThenParser(Parser<TIn> first, Parser<TIn> second)
        {
            _parsers = new[]{first, second};
        }


        public static ThenParser<TIn> Create(Parser<TIn> left, Parser<TIn> last)
        {
            return new ThenParser<TIn>(left, last);
        }

        public override Parser<TIn> Then(Parser<TIn> other)
        {
            var thenOther = other as ThenParser<TIn>;
            if (thenOther != null)
            {
                return new ThenParser<TIn>(_parsers.Concat(thenOther._parsers).ToArray());
            }

            return new ThenParser<TIn>(_parsers.Concat(new[]{other}).ToArray());
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            Result<string, Unit> result = null;
            for (int i = 0; i < _parsers.Length; ++i)
            {
                var parser = _parsers[i];
                result = parser.Run(input);
                if (result.IsFailure)
                    return result;
            }
            return result;
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as ThenParser<TIn>);
        }

        public bool Equals(ThenParser<TIn> other)
        {
            return other != null &&
                   _parsers.SequenceEqual(other._parsers);
        }
        public override string Name
        {
            get { return string.Join(", then ", _parsers.Select(x => x.Name)); }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _parsers.All(x => x.CanMatchWithoutConsumingInput); }
        }
    }

    internal sealed class ThenParser<TIn, TOut> : Parser<TIn, TOut>, IEquatable<ThenParser<TIn, TOut>>
    {
        private readonly Parser<TIn>[] _parsers;
        private readonly Parser<TIn, TOut> _last;

        private ThenParser(Parser<TIn>[] firsts, Parser<TIn, TOut> last)
        {
            _parsers = firsts;
            _last = last;
        }

        private ThenParser(Parser<TIn> first, Parser<TIn, TOut> second)
        {
            _parsers = new[] { first };
            _last = second;
        }

        public static ThenParser<TIn, TOut> Create(Parser<TIn> left, Parser<TIn, TOut> last)
        {
            return new ThenParser<TIn, TOut>(left, last);
        }

        public override Parser<TIn, TOut> Then(Parser<TIn, TOut> other)
        {

            var thenOther = other as ThenParser<TIn, TOut>;
            if (thenOther != null)
            {
                return new ThenParser<TIn, TOut>(_parsers.Concat(new[]{_last}).Concat(thenOther._parsers).ToArray(), thenOther._last);
            }

            return new ThenParser<TIn, TOut>(_parsers.Concat(new[] { _last }).ToArray(), other);
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            for (int i = 0; i < _parsers.Length; ++i)
            {
                var result = _parsers[i].Run(input);
                if (result.IsFailure)
                    return result;
            }
            return _last.Run(input);
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            for (int i = 0; i < _parsers.Length; ++i)
            {
                var result = _parsers[i].Run(input);
                string message;
                if (result.TryGetFailure(out message))
                    return new Failure<string, TOut>(message);
            }
            return _last.RunWithResult(input);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as ThenParser<TIn, TOut>);
        }

        public bool Equals(ThenParser<TIn, TOut> other)
        {
            return other != null &&
                   _parsers.SequenceEqual(other._parsers) &&
                   _last.Equals(other._last);
        }
        
        public override string Name
        {
            get { return string.Join(", then ", _parsers.Concat(new[]{_last}).Select(x=>x.Name)); }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _parsers.All(x => x.CanMatchWithoutConsumingInput) && _last.CanMatchWithoutConsumingInput; }
        }
    }
}