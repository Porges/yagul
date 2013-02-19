using System;
using System.Collections.Generic;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Many<TIn, TOut> : Parser<TIn, IList<TOut>>, IEquatable<Many<TIn, TOut>>
    {
        private readonly Parser<TIn, TOut> _parser;
        private readonly int _min;
        private readonly int _max;

        private Many(Parser<TIn, TOut> parser, int min, int max)
        {
            _parser = parser;
            _min = min;
            _max = max;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var saved = input.Save();
            var result = _parser.Run(input);

            while (result.IsSuccess)
            {
                if (++count == _max)
                    return default(Unit);

                saved = input.Save();
                result = _parser.Run(input);
            }

            if (count < _min)
                return string.Format("Expected at least '{0}' copies, only found '{1}'.", _min, count);

            // the last one failed so restore the position
            input.Restore(saved);
            return default(Unit);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Many<TIn, TOut>);
        }

        public override Result<string, IList<TOut>> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var list = new List<TOut>();
            var saved = input.Save();
            var result = _parser.RunWithResult(input);

            TOut success;
            while (result.TryGetSuccess(out success))
            {
                list.Add(success);

                if (++count == _max)
                    return list;

                saved = input.Save();
                result = _parser.RunWithResult(input);
            }

            if (count < _min)
                return string.Format("Expected at least '{0}' copies, only found '{1}'.", _min, count);

            // the last one failed so restore the position
            input.Restore(saved);
            return list;
        }

        public bool Equals(Many<TIn, TOut> other)
        {
            return other != null &&
                   Equals(_parser, other._parser) &&
                   _min == other._min &&
                   _max == other._max;
        }

        public static Parser<TIn, IList<TOut>> Create(Parser<TIn, TOut> parser, int min, int max)
        {
            if (min == 1 && max == 1)
                return parser.Select(x => (IList<TOut>) new[] {x});

            return new Many<TIn, TOut>(parser, min, max);
        }
    }

    internal sealed class Many<TIn> : Parser<TIn>, IEquatable<Many<TIn>>
    {
        private readonly Parser<TIn> _parser;
        private readonly int _min;
        private readonly int _max;

        private Many(Parser<TIn> parser, int min, int max)
        {
            _parser = parser;
            _min = min;
            _max = max;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var saved = input.Save();
            var result = _parser.Run(input);

            while (result.IsSuccess)
            {
                if (++count == _max)
                    return default(Unit);

                saved = input.Save();
                result = _parser.Run(input);
            }

            if (count < _min)
                return string.Format("Expected at least '{0}' copies, only found '{1}'.", _min, count);

            // the last one failed so restore the position
            input.Restore(saved);
            return default(Unit);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Many<TIn>);
        }

        public bool Equals(Many<TIn> other)
        {
            return other != null &&
                   Equals(_parser, other._parser) &&
                   _min == other._min &&
                   _max == other._max;
        }

        public static Parser<TIn> Create(Parser<TIn> parser, int min, int max)
        {
            if (min == 1 && max == 1)
                return parser;

            return new Many<TIn>(parser, min, max);
        }
    }
}