using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    public class Named<TIn> : Parser<TIn>
    {
        private readonly Parser<TIn> _parser;
        private readonly string _name;

        public Named(Parser<TIn> parser, string name)
        {
            _parser = parser;
            _name = name;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return _parser.Run(input);
        }

        public override string Name
        {
            get { return _name; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _parser.CanMatchWithoutConsumingInput; }
        }
    }
}
