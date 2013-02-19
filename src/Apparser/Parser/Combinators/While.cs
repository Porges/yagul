using System;
using System.Collections.Generic;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class While<T> : Parser<T, IList<T>>, IEquatable<While<T>>
    {
        private readonly Func<T, bool> _predicate;
        private readonly int _min;
        private readonly int _max;

        private While(Func<T, bool> predicate, int min, int max)
        {
            _predicate = predicate;
            _min = min;
            _max = max;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<T, TSave> input)
        {
            var count = 0;
            var saved = input.Save();

            while (input.MoveNext() && _predicate(input.Current))
            {
                if (++count == _max)
                    return default(Unit);

                saved = input.Save();
            }

            if (count < _min)
                return string.Format("Expected at least '{0}' copies, only found '{1}'.", _min, count);

            // the last one failed so restore the position
            input.Restore(saved);
            return default(Unit);
        }

        public override bool Equals(Parser<T> other)
        {
            return Equals(other as While<T>);
        }

        public override Result<string, IList<T>> RunWithResult<TSave>(IParserInput<T, TSave> input)
        {
            var count = 0;
            var list = new List<T>();
            var saved = input.Save();

            while (input.MoveNext() && _predicate(input.Current))
            {
                list.Add(input.Current);

                if (++count == _max)
                    return list;

                saved = input.Save();
            }

            if (count < _min)
                return string.Format("Expected at least '{0}' copies, only found '{1}'.", _min, count);

            // the last one failed so restore the position
            input.Restore(saved);
            return list;
        }

        public bool Equals(While<T> other)
        {
            return other != null &&
                   Equals(_predicate, other._predicate) &&
                   _min == other._min &&
                   _max == other._max;
        }

        public static Parser<T, IList<T>> Create(Func<T, bool> predicate, int min, int max)
        {
            return new While<T>(predicate, min, max);
        }
    }
}