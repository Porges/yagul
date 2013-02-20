using System;
using System.Collections.Generic;
using Apparser.Input;
using Apparser.Parser.Combinators;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser
{
    public abstract class Parser
    {
        public abstract Result<string, Unit> Run<TSave>(IParserInput input);
    }

    public abstract class Parser<TIn> : IEquatable<Parser<TIn>>
    {
        public abstract Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input) where TSave : IEquatable<TSave>;

        public static Parser<TIn> operator |(Parser<TIn> left, Parser<TIn> right)
        {
            return left.OrElse(right);
        }

        public abstract bool Equals(Parser<TIn> other);

        public virtual Parser<TIn> OrElse(Parser<TIn> other)
        {
            return EitherOf<TIn>.Create(this, other);
        }

        public virtual Parser<TIn> Many(int min = 0, int max = int.MaxValue)
        {
            return Many<TIn>.Create(this, min, max);
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
        public override Result<string, Unit> Run<TSave>(IParserInput<TIn, TSave> input)
        {
            return RunWithResult(input).SelectSuccess(_ => default(Unit));
        }

        public abstract Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn,TSave> input) where TSave : IEquatable<TSave>;

        public static Parser<TIn, TOut> operator |(Parser<TIn, TOut> left, Parser<TIn, TOut> right)
        {
            return left.OrElse(right);
        }

        public virtual new Parser<TIn, IList<TOut>> Many(int min = 0, int max = int.MaxValue)
        {
            return Many<TIn, TOut>.Create(this, min, max);
        }

        public virtual new Parser<TIn, TOut> OrElse(Parser<TIn, TOut> other)
        {
            return EitherOf<TIn, TOut>.Create(this, other);
        }
        
        ///// <summary>
        ///// We are allowed to make a parser with ignored output a parser returning any type.
        ///// 
        ///// This handles the fail type.
        ///// </summary>
        ///// <param name="noRight"></param>
        ///// <returns></returns>
        //public static implicit operator Parser<TIn, TOut>(Parser<TIn, Any> noRight)
        //{
        //    return noRight.Select<TIn, Any, TOut>(_ => { throw new InvalidOperationException(); });
        //}
        
        ///// <summary>
        ///// We are allowed to explicitly ignore any output.
        ///// </summary>
        ///// <param name="ignoreOutput">the parser which is to be ignored</param>
        ///// <returns></returns>
        //public static explicit operator Parser<TIn, Any>(Parser<TIn, TOut> ignoreOutput)
        //{
        //    return ignoreOutput.Select<TIn, TOut, Any>(_ => null);
        //}

        /// <summary>
        /// Explicitly ignore output.
        /// </summary>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public static Parser<TIn> operator ~(Parser<TIn, TOut> ignore)
        {
            return ignore;
        }

    }

    public static class ParserExtensions
    {
        public static Parser<T, T> Any<T>()
        {
            return AnyValue<T>.Create();
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
            return new Fail<T>(message);
        }

        public static Parser<T> Return<T>()
        {
            return Success<T>.Instance;
        }

        public static Parser<T, TValue> Return<T, TValue>(TValue value)
        {
            return new Combinators.Success<T, TValue>(value);
        }

        public static Parser<T, T> Exactly<T>(T item)
        {
            return new Exactly<T>(item);
        }

        public static Parser<T, T> Satisfy<T>(Func<T, bool> predicate)
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
            return EOFValue<TIn>.Instance;
        }

        public static Parser<TIn, TOut> ApplyTo<TIn, TMid, TOut>(this Func<TMid, TOut> projection,
                                                                 Parser<TIn, TMid> parser)
        {
            return new App<TIn,TMid,TOut>(projection, parser);
        }

        public static Parser<TIn, TFOut> ThenTo<TIn, TFIn, TFOut>(this Parser<TIn, Func<TFIn, TFOut>> previous, Parser<TIn, TFIn> next)
        {
            return previous.SelectMany(f => f.ApplyTo(next));
        }
    }
}
