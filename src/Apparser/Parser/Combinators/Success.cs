using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Success<TIn> : Parser<TIn>
    {
        private static readonly Success<TIn> _instance = new Success<TIn>();

        private Success()
        { }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return Unit.It;
        }

        public static Success<TIn> Instance
        {
            get { return _instance; }
        }
    }

    internal sealed class Success<TIn, TValue> : Parser<TIn, TValue>
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
    }
}
