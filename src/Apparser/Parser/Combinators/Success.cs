using System;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Success<TIn> : Parser<TIn>, IEquatable<Success<TIn>>
    {
        private static readonly Success<TIn> _instance = new Success<TIn>();

        private Success()
        { }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return default(Unit);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Success<TIn>);
        }

        public static Success<TIn> Instance
        {
            get { return _instance; }
        }

        public override string Name
        {
            get { return "success"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return true; }
        }

        public bool Equals(Success<TIn> other)
        {
            return other != null;
        }
    }

    internal sealed class Success<TIn, TValue> : Parser<TIn, TValue>, IEquatable<Success<TIn, TValue>>
    {
        private readonly TValue _value;

        public Success(TValue value)
        {
            _value = value;
        }

        public override Result<string, TValue> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            return _value;
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Success<TIn, TValue>);
        }

        public bool Equals(Success<TIn, TValue> other)
        {
            return other != null &&
                   Equals(_value, other._value);
        }
        public override string Name
        {
            get { return "success"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return true; }
        }
    }
}
