using System;
using System.Collections.Generic;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class TakeWhile<TIn> : Parser<TIn, IList<TIn>>, IEquatable<TakeWhile<TIn>>
    {
        private readonly Func<TIn, bool> _predicate;
        private readonly int _min;
        private readonly int _max;

        public TakeWhile(Func<TIn, bool> predicate, int min, int max)
        {
            _predicate = predicate;
            _min = min;
            _max = max;
        }

        public override Result<string, IList<TIn>> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var list = new List<TIn>();
            var saved = input.Save();

            var success = input.MoveNext() && _predicate(input.Current);

            while (success)
            {
                list.Add(input.Current);

                if (++count == _max)
                    return list;

                saved = input.Save();
                success = input.MoveNext() && _predicate(input.Current);
            }

            if (count < _min)
                return string.Format("Not Enough Times");

            // the last one failed so restore the position
            input.Restore(saved);
            return list;
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as TakeWhile<TIn>);
        }

        public override string Name
        {
            get { return "takeWhile"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _min == 0; }
        }

        public bool Equals(TakeWhile<TIn> other)
        {
            return other != null &&
                   _min == other._min &&
                   _max == other._max &&
                   _predicate.Equals(other._predicate);
        }
    }

    internal sealed class SkipWhile<TIn> : Parser<TIn>, IEquatable<SkipWhile<TIn>>
    {
        private readonly Func<TIn, bool> _predicate;
        private readonly int _min;
        private readonly int _max;

        public SkipWhile(Func<TIn, bool> predicate, int min, int max)
        {
            _predicate = predicate;
            _min = min;
            _max = max;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var saved = input.Save();
            var success = input.MoveNext() && _predicate(input.Current);

            while (success)
            {
                if (++count == _max)
                    return default(Unit);

                saved = input.Save();
                success = input.MoveNext() && _predicate(input.Current);
            }

            if (count < _min)
                return string.Format("Not Enough Times");

            // the last one failed so restore the position
            input.Restore(saved);
            return default(Unit);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as SkipWhile<TIn>);
        }

        public override string Name
        {
            get { return "takeWhile"; }
        }


        public override bool CanMatchWithoutConsumingInput
        {
            get { return  _min == 0; }
        }

        public bool Equals(SkipWhile<TIn> other)
        {
            return other != null &&
                   _min == other._min &&
                   _max == other._max &&
                   _predicate.Equals(other._predicate);
        }
    }
}