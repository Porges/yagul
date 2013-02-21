using System;
using System.Collections.Generic;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class SepBy<TIn, TOut> : Parser<TIn, IList<TOut>>, IEquatable<SepBy<TIn,TOut>>
    {
        private readonly Parser<TIn, TOut> _parser;
        private readonly Parser<TIn> _separator;
        private readonly int _min;
        private readonly int _max;

        public SepBy(Parser<TIn, TOut> parser, Parser<TIn> separator, int min, int max)
        {
            _parser = parser;
            _separator = separator;
            _min = min;
            _max = max;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var saved = input.Save();
            var result = _parser.Run(input);

            Unit success;
            while (result.TryGetSuccess(out success))
            {
                if (++count == _max)
                    return new Outcomes.Success<string, Unit>(success);

                saved = input.Save();
                if (_separator.Run(input).IsFailure)
                {
                    break;
                }

                result = _parser.Run(input);
            }

            if (count < _min)
            {
                string failureMessage;
                if (result.TryGetFailure(out failureMessage))
                {
                    return new Failure<string, Unit>(failureMessage);
                }

                return new Failure<string, Unit>(string.Format("Expected at least {0} copies of ({2}) (separated by {3}) only found {1}.", _min,
                                     count, _parser.Name, _separator.Name));
            }

            // the last one failed so restore the position
            input.Restore(saved);
            return new Outcomes.Success<string, Unit>(success);
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
                    return new Outcomes.Success<string, IList<TOut>>(list);

                saved = input.Save();
                if (_separator.Run(input).IsFailure)
                {
                    break;
                }

                result = _parser.RunWithResult(input);
            }

            if (count < _min)
            {
                string failureMessage;
                if (result.TryGetFailure(out failureMessage))
                {
                    return new Failure<string, IList<TOut>>(failureMessage);
                }

                return
                    new Failure<string, IList<TOut>>(string.Format(
                        "Expected at least {0} copies of ({2}) (separated by {3}) only found {1}. Failed because: {2}",
                        _min, count, _parser.Name, _separator.Name));
            }

            // the last one failed so restore the position
            input.Restore(saved);
            return new Outcomes.Success<string, IList<TOut>>(list);
        }


        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as SepBy<TIn,TOut>);
        }


        public bool Equals(SepBy<TIn, TOut> other)
        {
            return other != null &&
                   _min == other._min &&
                   _max == other._max &&
                   _parser.Equals(other._parser) &&
                   _separator.Equals(other._separator);
        }

        public override string Name
        {
            get
            {
                var inner = _parser.Name;
                var sepBy = _separator.Name;

                if (_min == 0)
                {
                    if (_max == int.MaxValue)
                    {
                        return string.Format("({0}), separated by {1}, any number of times", inner, sepBy);
                    }
                    else
                    {
                        return string.Format("({0}), separated by {2}, up to {1} times", inner, _max, sepBy);
                    }
                }
                else
                {
                    if (_max == int.MaxValue)
                    {

                        return string.Format("({0}), separated by {2} at least {1} times", inner, _min, sepBy);
                    }
                    else
                    {
                        return string.Format("({0}), separated by {3}, from {1} to {2} times", inner, _min, _max, sepBy);

                    }
                }
            }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _min == 0 || (_separator.CanMatchWithoutConsumingInput && _parser.CanMatchWithoutConsumingInput); }
        }

    }

    internal sealed class SepBy<TIn> : Parser<TIn>, IEquatable<SepBy<TIn>>
    {
        private readonly Parser<TIn> _parser;
        private readonly Parser<TIn> _separator;
        private readonly int _min;
        private readonly int _max;

        public SepBy(Parser<TIn> parser, Parser<TIn> separator, int min, int max)
        {
            _parser = parser;
            _separator = separator;
            _min = min;
            _max = max;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var saved = input.Save();
            var result = _parser.Run(input);

            Unit success;
            while (result.TryGetSuccess(out success))
            {
                if (++count == _max)
                    return new Outcomes.Success<string, Unit>(success);

                saved = input.Save();
                if (_separator.Run(input).IsFailure)
                {
                    break;
                }

                result = _parser.Run(input);
            }

            if (count < _min)
            {
                string failureMessage;
                if (result.TryGetFailure(out failureMessage))
                {
                    return new Failure<string, Unit>(failureMessage);
                }

                return new Failure<string, Unit>(string.Format("Expected at least {0} copies of ({2}) (separated by {3}) only found {1}.", _min,
                                     count, _parser.Name, _separator.Name));
            }

            // the last one failed so restore the position
            input.Restore(saved);
            return new Outcomes.Success<string, Unit>(success);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as SepBy<TIn>);
        }

        public bool Equals(SepBy<TIn> other)
        {
            return other != null &&
                   _min == other._min &&
                   _max == other._max &&
                   _parser.Equals(other._parser) &&
                   _separator.Equals(other._separator);
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
                        return string.Format("({0}), any number of times", inner);
                    }
                    else
                    {
                        return string.Format("({0}), up to {1} times", inner, _max);
                    }
                }
                else
                {
                    if (_max == int.MaxValue)
                    {

                        return string.Format("({0}), at least {1} times", inner, _min);
                    }
                    else
                    {
                        return string.Format("({0}), from {1} to {2} times", inner, _min, _max);

                    }
                }
            }
        }


        public override bool CanMatchWithoutConsumingInput
        {
            get { return _min == 0 || (_separator.CanMatchWithoutConsumingInput && _parser.CanMatchWithoutConsumingInput); }
        }
    }
}