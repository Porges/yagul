using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class AnyValue<TIn> : Parser<TIn,TIn>
    {
        private AnyValue()
        {}

        public override Result<string, TIn> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            if (input.MoveNext())
                return input.Current;

            return "No value available";
        }

        static readonly AnyValue<TIn> _instance = new AnyValue<TIn>();
        public static AnyValue<TIn> Instance
        {
            get { return _instance; }
        }

    }
}