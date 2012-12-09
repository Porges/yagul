using System;
using System.Collections.Generic;
using Outcomes;
using Yagul.Extensions;
using Yagul.Types;

namespace Apparser
{
    public interface IParserInput<out T, TSave> : IEnumerator<T>
    {
        TSave Save();
        void Restore(TSave save);
    }

    public abstract class Parser<TIn>
    {
        public abstract Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input);

        public static Parser<TIn> operator |(Parser<TIn> left, Parser<TIn> right)
        {
            return new EitherOf<TIn>(left, right);
        }
    }
    
    public abstract class Parser<TIn, TOut> : Parser<TIn>
    {
        /// <summary>
        /// Runs a <see cref="Parser{TIn, TOut}"/> as if it were a <see cref="Parser{TIn}"/> by ignoring
        /// the output value.
        /// </summary>
        /// <typeparam name="TSave"></typeparam>
        /// <param name="input">The input source.</param>
        /// <returns>A <see cref="Result{TFail,TSuccess}"/> representing the parse result.</returns>
        public sealed override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return RunWithResult(input).SelectSuccess(_ => Unit.It);
        }

        public abstract Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn,TSave> input);

        public static Parser<TIn, TOut> operator |(Parser<TIn, TOut> left, Parser<TIn, TOut> right)
        {
            return new EitherOf<TIn, TOut>(left, right);
        }

        /// <summary>
        /// We are allowed to make a parser with ignored output a parser returning any type.
        /// 
        /// This handles the fail type.
        /// </summary>
        /// <param name="noRight"></param>
        /// <returns></returns>
        public static implicit operator Parser<TIn, TOut>(Parser<TIn, Any> noRight)
        {
            return noRight.Select<TIn, Any, TOut>(_ => { throw new InvalidOperationException(); });
        }

        /// <summary>
        /// We are allowed to explicitly ignore any output.
        /// </summary>
        /// <param name="ignoreOutput">the parser which is to be ignored</param>
        /// <returns></returns>
        public static explicit operator Parser<TIn, Any>(Parser<TIn, TOut> ignoreOutput)
        {
            return ignoreOutput.Select<TIn, TOut, Any>(_ => null);
        }

        public static Parser<TIn> operator ~(Parser<TIn, TOut> ignore)
        {
            return ignore;
        }

    }

    public static class Parser
    {
        public static Parser<T, T> Any<T>()
        {
            return AnyOf<T>.Instance;
        }

        public static Parser<T, TSeq> ExactSequence<TSeq, T>(TSeq items)
            where TSeq : IEnumerable<T>
        {
            var enumerator = items.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new ArgumentNullException("items");
            }

            var parser = Exactly(enumerator.Current);
            while (enumerator.MoveNext())
            {
                parser = parser.Then(Exactly(enumerator.Current));
            }

            return parser.Select(_ => items);
        }

        public static Parser<TIn, TOut> Select<TIn, TMid, TOut>(this Parser<TIn, TMid> parser,
                                                                Func<TMid, TOut> projection)
        {
            return new App<TIn, TMid, TOut>(projection, parser);
        }

        public static Parser<TIn, TOut> SelectMany<TIn, TMid, TOut>(this Parser<TIn, TMid> parser,
            Func<TMid, Parser<TIn, TOut>> projection)
        {
            return new Bind<TIn, TMid, TOut>(projection, parser);
        }

        public static Parser<TIn, TFinal> SelectMany<TIn, TMid, TOut, TFinal>(this Parser<TIn, TMid> parser,
            Func<TMid, Parser<TIn, TOut>> projection, Func<TMid, TOut, TFinal> nextProjection)
        {
            return parser.SelectMany(x => projection(x).Select(y => nextProjection(x, y)));
        }

        public static Parser<T> Fail<T>(string message)
        {
            return new Fail<T> (message);
        }

        public static Parser<T, T> Exactly<T>(T item)
        {
            return new Exactly<T>(item);
        }

        public static Parser<T, T> Satisfy<T>(Predicate<T> predicate)
        {
            return new Satisfy<T>(predicate);
        }


        public static Parser<TIn> Then<TIn>(this Parser<TIn> first, Parser<TIn> second)
        {
            return new Then<TIn>(first, second);
        }

        public static Parser<TIn, TOut> Then<TIn, TOut>(this Parser<TIn> first, Parser<TIn, TOut> second)
        {
            return new Then<TIn, TOut>(first, second);
        }

        public static Parser<TIn> EOF<TIn>()
        {
            return EOFOf<TIn>.Instance;
        }

        public static Parser<TIn, IList<TOut>> Many<TIn, TOut>(this Parser<TIn, TOut> parser)
        {
            return new Many<TIn, TOut>(parser);
        }

        public static Parser<TIn, TOut> ApplyTo<TIn, TMid, TOut>(this Func<TMid, TOut> projection,
                                                                 Parser<TIn, TMid> parser)
        {
            return new App<TIn,TMid,TOut>(projection, parser);
        }

        public static Parser<TIn, IList<TOut>> Many1<TIn, TOut>(this Parser<TIn, TOut> parser)
        {
            Func<TOut, IList<TOut>, IList<TOut>> inserter = (x, xs) =>
            {
                xs.Insert(0, x);
                return xs;
            };
            return inserter.Curry().ApplyTo(parser).SelectMany(f => f.ApplyTo(Many(parser)));
        }

        public static Parser<TIn, TFOut> ThenTo<TIn, TFIn, TFOut>(this Parser<TIn, Func<TFIn, TFOut>> previous, Parser<TIn, TFIn> next)
        {
            return previous.SelectMany(f => f.ApplyTo(next));
        }
    }

    public class Bind<TIn, TMid, TOut> : Parser<TIn, TOut>
    {
        private readonly Func<TMid, Parser<TIn, TOut>> _projection;
        private readonly Parser<TIn, TMid> _parser;

        public Bind(Func<TMid, Parser<TIn, TOut>> projection, Parser<TIn, TMid> parser)
        {
            _projection = projection;
            _parser = parser;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            return _parser
                .RunWithResult(input)
                .SelectMany(x => _projection(x).RunWithResult(input));
        }
    }

    public class Many<TIn, TOut> : Parser<TIn, IList<TOut>>
    {
        private readonly Parser<TIn, TOut> _parser;

        public Many(Parser<TIn, TOut> parser)
        {
            _parser = parser;
        }
        
        public override Result<string, IList<TOut>> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var list = new List<TOut>();
            var saved = input.Save();
            var result = _parser.RunWithResult(input);

            TOut success;
            while (result.TryGetSuccess(out success))
            {
                list.Add(success);
                saved = input.Save();
                result = _parser.RunWithResult(input);
            }

            input.Restore(saved);
            return list;
        }
    }

    public sealed class EOFOf<TIn> : Parser<TIn>
    {
        private static readonly EOFOf<TIn> _instance = new EOFOf<TIn>();

        private EOFOf() { }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn,TSave> input)
        {
            if (input.MoveNext())
                return "Expected end of input.";

            return Unit.It;
        }

        public static EOFOf<TIn> Instance
        {
            get { return _instance; }
        }
    }

    public class AppF2<TIn, TMid1, TMid2, TOut> : Parser<TIn, TOut>
    {
        private readonly Parser<TIn, TMid1> _parser1;
        private readonly Parser<TIn, TMid2> _parser2;
        private readonly Func<TMid1, TMid2, TOut> _projection;

        public AppF2(Func<TMid1, TMid2, TOut> projection, Parser<TIn, TMid1> parser1, Parser<TIn, TMid2> parser2)
        {
            _projection = projection;
            _parser1 = parser1;
            _parser2 = parser2;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            return _parser1
                .RunWithResult(input)
                .SelectMany(_ => _parser2.RunWithResult(input).Select(__ => Tuple.Create(_, __)))
                .Select(_ => _projection(_.Item1, _.Item2));
        }
    }

    public class App<TIn, TMid, TOut> : Parser<TIn, TOut>
    {
        private readonly Parser<TIn, TMid> _parser;
        private readonly Func<TMid, TOut> _projection;

        public App(Func<TMid, TOut> projection, Parser<TIn, TMid> parser)
        {
            _projection = projection;
            _parser = parser;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            return _parser.RunWithResult(input).Select(_projection);
        }
    }

    public class Then<TIn, TOut> : Parser<TIn, TOut>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn, TOut> _second;

        public Then(Parser<TIn> first, Parser<TIn, TOut> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            return _first
                .Run(input)
                .SelectMany(_ => _second.RunWithResult(input));
        }
    }

    public class Then<TIn> : Parser<TIn>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn> _second;

        public Then(Parser<TIn> first, Parser<TIn> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return _first
                .Run(input)
                .SelectMany(_ => _second.Run(input));
        }
    }

    public class EitherOf<TIn> : Parser<TIn>
    {
        private readonly Parser<TIn> _first;
        private readonly Parser<TIn> _second;

        public EitherOf(Parser<TIn> first, Parser<TIn> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            return _first.Run(input).SelectManyFailure(_ =>
                {
                    input.Restore(saved);
                    return _second.Run(input);
                });
        }
    }

    public class EitherOf<TIn, TOut> : Parser<TIn, TOut>
    {
        private readonly Parser<TIn, TOut> _first;
        private readonly Parser<TIn, TOut> _second;

        public EitherOf(Parser<TIn, TOut> first, Parser<TIn, TOut> second)
        {
            _first = first;
            _second = second;
        }

        public override Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn, TSave> input)
        {
            var saved = input.Save();
            return _first.RunWithResult(input).SelectManyFailure(_ =>
                {
                    input.Restore(saved);
                    return _second.RunWithResult(input);
                });
        }
    }
    
    public sealed class Satisfy<T> : Parser<T, T>
    {
        private readonly Predicate<T> _predicate;

        public Satisfy(Predicate<T> predicate)
        {
            _predicate = predicate;
        } 

        public override Result<string, T> RunWithResult<TSave>(IParserInput<T,TSave> input)
        {
            if (!input.MoveNext())
                return "Unexpected end of input.";

            if (_predicate(input.Current))
                return input.Current;

            return "Could not satisfy predicate.";
        }
    }

    public sealed class Exactly<TIn> : Parser<TIn, TIn>
    {
        private readonly TIn _item;

        public Exactly(TIn item)
        {
            _item = item;
        }

        public override Result<string, TIn> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            if (!input.MoveNext())
                return string.Format("Unexpected end of input.");

            if (input.Current.Equals(_item))
                return _item;

            return string.Format("Expected '{0}', got '{1}'.", _item, input.Current);
        }
    }

    public sealed class AnyOf<TIn> : Parser<TIn,TIn>
    {
        private AnyOf()
        {}

        public override Result<string, TIn> RunWithResult<TSave>(IParserInput<TIn,TSave> input)
        {
            if (input.MoveNext())
                return input.Current;

            return "No value available";
        }

        static readonly AnyOf<TIn> _instance = new AnyOf<TIn>();
        public static AnyOf<TIn> Instance
        {
            get { return _instance; }
        }

    }
    
    public class Fail<T> : Parser<T>
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
