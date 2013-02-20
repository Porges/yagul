using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Then<TIn> : Parser<TIn>, IEquatable<Then<TIn>>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn> _second;

        public Then(Parser<TIn> first, Parser<TIn> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return _first
                .Run(input)
                .SelectManySuccess(_ => _second.Run(input));
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Then<TIn>);
        }

        public bool Equals(Then<TIn> other)
        {
            return other != null &&
                   Equals(_first, other._first) &&
                   Equals(_second, other._second);
        }
        public override string Name
        {
            get { return string.Format("{0}, then {1}", _first.Name, _second.Name); }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _first.CanMatchWithoutConsumingInput && _second.CanMatchWithoutConsumingInput; }
        }
    }

    internal sealed class Then<TIn, TOut> : Parser<TIn, TOut>, IEquatable<Then<TIn, TOut>>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn, TOut> _second;

        public Then(Parser<TIn> first, Parser<TIn, TOut> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            return _first
                .Run(input)
                .SelectManySuccess(_ => _second.RunWithResult(input));
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Then<TIn, TOut>);
        }

        public bool Equals(Then<TIn, TOut> other)
        {
            return other != null &&
                   Equals(_first, other._first) &&
                   Equals(_second, other._second);
        }
        
        public override string Name
        {
            get { return string.Format("{0}, then {1}", _first.Name, _second.Name); }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _first.CanMatchWithoutConsumingInput && _second.CanMatchWithoutConsumingInput; }
        }
    }
}