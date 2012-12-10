using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    internal sealed class Fail<T> : Parser<T>
    {
        private readonly string _message;

        public Fail(string message)
        {
            _message = message;
        }
        
        public string Message
        {
            get { return _message; }
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<T,TSave> input)
        {
            return _message;
        }
    }
}