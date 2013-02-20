using System;
using System.Collections.Generic;
using Apparser.Input;
using Outcomes;

namespace Apparser.Parser.Combinators
{
    internal sealed class Until<TIn, TOut> : Parser<TIn, IList<TOut>>, IEquatable<Until<TIn, TOut>>
    {
        private readonly Parser<TIn, TOut> _repeat;
        private readonly Parser<TIn> _end;

        public Until(Parser<TIn, TOut> repeat, Parser<TIn> end)
        {
            _repeat = repeat;
            _end = end;
        }

        public override Result<string, IList<TOut>> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var list = new List<TOut>();

            var result = _end.Run(input);

            string message;
            for (var save = input.Save(); result.TryGetFailure(out message); save = input.Save())
            {
                input.Restore(save);

                var repeated = _repeat.RunWithResult(input);

                TOut success;
                if (!repeated.TryGetSuccess(out success))
                {
                    repeated.TryGetFailure(out message);
                    return message;
                }

                list.Add(success);
            }

            return list;
        }

        public override bool Equals(Parser<TIn> other)
        {
            return Equals(other as Until<TIn, TOut>);
        }

        public bool Equals(Until<TIn, TOut> other)
        {
            return other != null &&
                   Equals(_end, other._end) &&
                   Equals(_repeat, other._repeat);
        }
        public override string Name
        {
            get { return "until"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return _end.CanMatchWithoutConsumingInput; }
        }
    }
}
