using System.Collections.Generic;
using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class Many<TIn, TOut> : Parser<TIn, IList<TOut>>
    {
        private readonly Parser<TIn, TOut> _parser;
        private readonly int _min;
        private readonly int _max;

        public Many(Parser<TIn, TOut> parser, int min, int max)
        {
            _parser = parser;
            _min = min;
            _max = max;
        }

        public override Result<string, IList<TOut>> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var count = 0;
            var list = new List<TOut>();
            var saved = input.Save();
            var result = _parser.RunWithResult(input);

            TOut success;
            while (result.TryGetSuccess(out success))
            {
                list.Add(success);

                if (++count == _max)
                    return list;

                saved = input.Save();
                result = _parser.RunWithResult(input);
            }

            if (count < _min)
                return string.Format("Expected at least '{0}' copies, only found '{1}'.", _min, count);

            // the last one failed so restore the position
            input.Restore(saved);
            return list;
        }
    }
}