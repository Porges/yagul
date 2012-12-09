using Outcomes;
using Yagul.Types;

namespace Apparser
{
    public sealed class Success<TIn> : Parser<TIn>
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
}
