using System;
using System.Collections.Generic;
using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class Satisfy<T> : Parser<T, T>, IEquatable<Satisfy<T>>
    {
        private readonly Func<T, bool> _predicate;

        public Satisfy(Func<T, bool> predicate)
        {
            _predicate = predicate;
        } 

        public override Result<string, T> RunWithResult<TSave>(IParserInput<T,TSave> input)
        {
            if (!input.MoveNext())
                return "Unexpected end of input.";

            if (_predicate(input.Current))
                return input.Current;

            return "Could not satisfy predicate.";
        }
        
        public override Parser<T, IList<T>> Many(int min, int max)
        {
            if (min == 1 && max == 1)
                return this.Select(x => (IList<T>)new[]{x});

            return While<T>.Create(_predicate, min, max);
        }

        public override bool Equals(Parser<T> other)
        {
            return Equals(other as Satisfy<T>);
        }

        public bool Equals(Satisfy<T> other)
        {
            return other != null &&
                   Equals(_predicate, other._predicate);
        }
    }
}