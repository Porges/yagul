using System;
using System.Collections.Generic;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Many<TIn, TOut, TAgg> : Parser<TIn, TAgg>, IEquatable<Many<TIn, TOut, TAgg>>
    {
        private readonly Parser<TIn, TOut> _parser;
        private readonly int _min;
        private readonly int _max;
        private readonly Func<TAgg> _initialValue;
        private readonly Func<TAgg, TOut, TAgg> _aggregator;

        private Many(Parser<TIn, TOut> parser, int min, int max, Func<TAgg> initialValue, Func<TAgg, TOut, TAgg> aggregator)
        {
            if (parser.CanMatchWithoutConsumingInput && max > 1)
                throw new ArgumentException("You cannot use nonconsuming parsers with Many");

            _parser = parser;
            _min = min;
            _max = max;
            _initialValue = initialValue;
            _aggregator = aggregator;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var saved = input.Save();
            var result = _parser.Run(input);

            while (result.IsSuccess)
            {
                if (++count == _max)
                    return new Outcomes.Success<string, Unit>(default(Unit));

                saved = input.Save();
                result = _parser.Run(input);
            }

            if (count < _min)
                return new Failure<string, Unit>("");//string.Format("Expected at least '{0}' copies, only found '{1}'.", _min, count));

            // the last one failed so restore the position
            input.Restore(saved);
            return new Outcomes.Success<string, Unit>(default(Unit));
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Many<TIn, TOut, TAgg>);
        }

        public override Result<string, TAgg> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var list = _initialValue();
            var saved = input.Save();
            var result = _parser.RunWithResult(input);

            TOut success;
            while (result.TryGetSuccess(out success))
            {
                list = _aggregator(list, success);

                if (++count == _max)
                    return new Outcomes.Success<string, TAgg>(list);

                saved = input.Save();
                result = _parser.RunWithResult(input);
            }

            if (count < _min)
                return new Failure<string, TAgg>("");//string.Format("Expected at least {0} copies of ({2}), only found {1}.", _min, count, _parser.Name));

            // the last one failed so restore the position
            input.Restore(saved);
            return new Outcomes.Success<string, TAgg>(list);
        }

        public bool Equals(Many<TIn, TOut, TAgg> other)
        {
            return other != null &&
                   Equals(_parser, other._parser) &&
                   _min == other._min &&
                   _max == other._max;
        }

        public static Parser<TIn, TAgg> Create(Parser<TIn, TOut> parser, int min, int max, Func<TAgg> initialValue, Func<TAgg,TOut,TAgg> aggregator)
        {
            return new Many<TIn, TOut, TAgg>(parser, min, max, initialValue, aggregator);
        }

        public override string Name
        {
            get
            {
                var inner = _parser.Name;

                if (_min == 0)
                {
                    if (_max == int.MaxValue)
                    {
                        return string.Format("{0}, any number of times", inner);
                    }
                    else
                    {
                        return string.Format("{0}, up to {1} times", inner, _max);
                    }
                }
                else
                {
                    if (_max == int.MaxValue)
                    {

                        return string.Format("{0}, at least {1} times", inner, _min);
                    }
                    else
                    {
                        return string.Format("{0}, from {1} to {2} times", inner, _min, _max);
                        
                    }
                }
            }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _parser.CanMatchWithoutConsumingInput || _min == 0; }
        }
    }

    internal sealed class Many<TIn> : Parser<TIn>, IEquatable<Many<TIn>>
    {
        private readonly Parser<TIn> _parser;
        private readonly int _min;
        private readonly int _max;

        private Many(Parser<TIn> parser, int min, int max)
        {
            if (parser.CanMatchWithoutConsumingInput && max > 1)
                throw new ArgumentException("You cannot use nonconsuming parsers with Many");

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
                    return new Outcomes.Success<string, Unit>(default(Unit));

                saved = input.Save();
                result = _parser.Run(input);
            }

            if (count < _min)
                return new Failure<string, Unit>("");//string.Format("Expected at least {0} copies of ({2}), only found {1}.", _min, count, _parser.Name));

            // the last one failed so restore the position
            input.Restore(saved);
            return new Outcomes.Success<string, Unit>(default(Unit));
        }

        public override string Name
        {
            get
            {
                var inner = _parser.Name; 
                if (_min == 0)
                {
                    if (_max == int.MaxValue)
                    {
                        return string.Format("{0}, any number of times", inner);
                    }
                    else
                    {
                        return string.Format("{0}, up to {1} times", inner, _max);
                    }
                }
                else
                {
                    if (_max == int.MaxValue)
                    {

                        return string.Format("{0}, at least {1} times", inner, _min);
                    }
                    else
                    {
                        return string.Format("{0}, from {1} to {2} times", inner, _min, _max);

                    }
                }
            }
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
        
        public override bool CanMatchWithoutConsumingInput
        {
            get { return _parser.CanMatchWithoutConsumingInput || _min == 0; }
        }
    }
}