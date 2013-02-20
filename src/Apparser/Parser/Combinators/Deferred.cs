using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    public sealed class Deferred<TIn> : Parser<TIn>
    {
        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return Parser.Run(input);
        }

        public override bool Equals(Parser<TIn> other)
        {
            return false;
        }

        public Parser<TIn> Parser { get; set; }

        public override string Name
        {
            get { return "deferred"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return Parser != null && Parser.CanMatchWithoutConsumingInput; }
        }
    }

    public sealed class Deferred<TIn, TOut> : Parser<TIn, TOut>
    {
        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            return Parser.RunWithResult(input);
        }

        public Parser<TIn, TOut> Parser { get; set; }

        public override bool Equals(Parser<TIn> other)
        {
            return false;
        }

        public override string Name
        {
            get { return "deferred"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return Parser != null && Parser.CanMatchWithoutConsumingInput; }
        }
    }
}
