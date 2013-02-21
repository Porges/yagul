using System;
using System.Collections.Generic;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

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
                    return new Failure<string, IList<TOut>>(message);
                }

                list.Add(success);
            }

            return new Outcomes.Success<string, IList<TOut>>(list);
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var result = _end.Run(input);

            string message;
            for (var save = input.Save(); result.TryGetFailure(out message); save = input.Save())
            {
                input.Restore(save);

                var repeated = _repeat.Run(input);

                if (repeated.TryGetFailure(out message))
                {
                    return new Failure<string, Unit>(message);
                }
            }

            return new Outcomes.Success<string, Unit>(default(Unit));
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
