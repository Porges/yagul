using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class EitherOf<TIn, TOut> : Parser<TIn, TOut>, IEquatable<EitherOf<TIn, TOut>>
    {
        private readonly Parser<TIn, TOut> _first;
        private readonly Parser<TIn, TOut> _second;

        public EitherOf(Parser<TIn, TOut> first, Parser<TIn, TOut> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            return _first
                .RunWithResult(input)
                .SelectManyFailure(_ =>
                {
                    input.Restore(saved);
                    return _second.RunWithResult(input);
                });
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as EitherOf<TIn, TOut>);
        }

        public bool Equals(EitherOf<TIn, TOut> other)
        {
            return other != null &&
                ((Equals(_first, other._first) && Equals(_second, other._second)) ||
                (Equals(_first, other._second) && Equals(_second, other._first)));
        }

        public static Parser<TIn, TOut> Create(Parser<TIn, TOut> left, Parser<TIn, TOut> right)
        {
            if (Equals(left, right))
                return left;

            var eitherLeft = left as EitherOf<TIn, TOut>;
            if (eitherLeft != null)
            {

            }

            var eitherRight = right as EitherOf<TIn, TOut>;
            if (eitherRight != null)
            {

            }

            return new EitherOf<TIn, TOut>(left, right);
        }
        public override string Name
        {
            get { return string.Format("either: ({0}), or: ({1})", _first.Name, _second.Name); }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _first.CanMatchWithoutConsumingInput || _second.CanMatchWithoutConsumingInput; }
        }
    }

    internal sealed class EitherOf<TIn> : Parser<TIn>, IEquatable<EitherOf<TIn>>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn> _second;

        private EitherOf(Parser<TIn> first, Parser<TIn> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            
            return _first
                .Run(input)
                .SelectManyFailure(_ =>
                {
                    input.Restore(saved);
                    return _second.Run(input);
                });
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as EitherOf<TIn>);
        }

        public static Parser<TIn> Create(Parser<TIn> left, Parser<TIn> right)
        {
            if (Equals(left, right))
                return left;

            var eitherLeft = left as EitherOf<TIn>;
            if (eitherLeft != null)
            {
                
            }

            var eitherRight = right as EitherOf<TIn>;
            if (eitherRight != null)
            {
                
            }

            return new EitherOf<TIn>(left, right);
        }

        public bool Equals(EitherOf<TIn> other)
        {
            return other != null &&
                   ((Equals(_first, other._first) && Equals(_second, other._second)) ||
                    (Equals(_second, other._first) && Equals(_first, other._second)));
        }
        public override string Name
        {
            get { return string.Format("either: ({0}), or: ({1})", _first.Name, _second.Name); }
        }


        public override bool CanMatchWithoutConsumingInput
        {
            get { return _first.CanMatchWithoutConsumingInput || _second.CanMatchWithoutConsumingInput; }
        }
    }
}