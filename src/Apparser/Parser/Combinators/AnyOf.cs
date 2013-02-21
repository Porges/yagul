using System;
using System.Collections.Generic;
using System.Linq;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class AnyOf<TIn, TOut> : Parser<TIn, TOut>, IEquatable<AnyOf<TIn, TOut>>
    {
        private readonly Parser<TIn, TOut>[] _parsers;

        private AnyOf(Parser<TIn, TOut>[] parsers)
        {
            _parsers = parsers;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            for (int i = 0; i < _parsers.Length; ++i)
            {
                var result = _parsers[i].Run(input);
                if (result.IsSuccess)
                {
                    return result;
                }
                input.Restore(saved);
            }

            return new Failure<string, Unit>("");
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            for (int i = 0; i < _parsers.Length; ++i)
            {
                var result = _parsers[i].RunWithResult(input);
                if (result.IsSuccess)
                {
                    return result;
                }
                input.Restore(saved);
            }

            return new Failure<string, TOut>("");
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as AnyOf<TIn, TOut>);
        }

        public bool Equals(AnyOf<TIn, TOut> other)
        {
            return other != null &&
                _parsers.SequenceEqual(other._parsers);
        }

        public static Parser<TIn, TOut> Create(Parser<TIn, TOut> left, Parser<TIn, TOut> right)
        {
            if (Equals(left, right))
                return left;

            var parser = new List<Parser<TIn, TOut>>();

            var eitherLeft = left as AnyOf<TIn, TOut>;
            if (eitherLeft != null)
            {
                parser.AddRange(eitherLeft._parsers);
            }
            else
            {
                parser.Add(left);
            }

            var eitherRight = right as AnyOf<TIn, TOut>;
            if (eitherRight != null)
            {
                parser.AddRange(eitherRight._parsers);
            }
            else
            {
                parser.Add(right);
            }

            return new AnyOf<TIn, TOut>(parser.ToArray());
        }

        public override string Name
        {
            get { return string.Join(", or", _parsers.Select(x=>x.Name)); }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _parsers.Any(x => x.CanMatchWithoutConsumingInput); }
        }
    }

    internal sealed class AnyOf<TIn> : Parser<TIn>, IEquatable<AnyOf<TIn>>
    {
        private readonly Parser<TIn>[] _parsers;

        private AnyOf(Parser<TIn>[] parsers)
        {
            _parsers = parsers;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            for (int i = 0; i < _parsers.Length; ++i)
            {
                var result = _parsers[i].Run(input);
                if (result.IsSuccess)
                {
                    return result;
                }
                input.Restore(saved);
            }

            return new Failure<string, Unit>("");
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as AnyOf<TIn>);
        }

        public static Parser<TIn> Create(Parser<TIn> left, Parser<TIn> right)
        {
            if (Equals(left, right))
                return left;

            var parser = new List<Parser<TIn>>();

            var eitherLeft = left as AnyOf<TIn>;
            if (eitherLeft != null)
            {
                parser.AddRange(eitherLeft._parsers);
            }
            else
            {
                parser.Add(left);
            }

            var eitherRight = right as AnyOf<TIn>;
            if (eitherRight != null)
            {
                parser.AddRange(eitherRight._parsers);
            }
            else
            {
                parser.Add(right);
            }

            return new AnyOf<TIn>(parser.ToArray());
        }

        public bool Equals(AnyOf<TIn> other)
        {
            return other != null &&
                   _parsers.SequenceEqual(other._parsers);
        }
        public override string Name
        {
            get { return string.Join(", or", _parsers.Select(x => x.Name)); }
        }


        public override bool CanMatchWithoutConsumingInput
        {
            get { return _parsers.Any(x => x.CanMatchWithoutConsumingInput); }
        }
    }
}