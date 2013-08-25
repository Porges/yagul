using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Apparser.Input;
using Apparser.Parser.Combinators;
using Outcomes;
using Yagul;
using Yagul.Types;

namespace Apparser.Parser
{
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
            return AnyOf<TIn>.Create(this, other);
        }

        public virtual Parser<TIn> Many(int min = 0, int max = int.MaxValue)
        {
            return Many<TIn>.Create(this, min, max);
        }

        public virtual Parser<TIn> Then(Parser<TIn> other)
        {
            return ThenParser<TIn>.Create(this, other);
        }

        public virtual Parser<TIn, TOut> Then<TOut>(Parser<TIn, TOut> other)
        {
            return ThenParser<TIn, TOut>.Create(this, other);
        }

        public static implicit operator Parser<TIn>(TIn something)
        {
            return Parser.Exactly(something);
        }

        public abstract string Name { get; }
        public abstract bool CanMatchWithoutConsumingInput { get; }
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
        public abstract Result<string, TOut> RunWithResult<TSave>(IParserInput<TIn,TSave> input) where TSave : IEquatable<TSave>;

        public static Parser<TIn, TOut> operator |(Parser<TIn, TOut> left, Parser<TIn, TOut> right)
        {
            return left.OrElse(right);
        }

        public virtual Parser<TIn, TAgg> Many<TAgg>(int min, int max, Func<TAgg> agg, Func<TAgg, TOut, TAgg> aggregation)
        {
            return Many<TIn, TOut, TAgg>.Create(this, min, max, agg, aggregation);
        }

        public virtual Parser<TIn, TOut> OrElse(Parser<TIn, TOut> other)
        {
            return AnyOf<TIn, TOut>.Create(this, other);
        }

        public virtual Parser<TIn, TOut> Then(Parser<TIn, TOut> other)
        {
            return ThenParser<TIn, TOut>.Create(this, other);
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

    /// <summary>
    /// Smart constructors for parsers.
    /// </summary>
    public static class Parser
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

        public static Parser<TIn> Ignore<TIn,TOut>(this Parser<TIn, TOut> parser)
        {
            return parser;
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
            return new Fail<T, Unit>(message);
        }

        public static Parser<TIn, IList<TOut>> ManyList<TIn, TOut>(this Parser<TIn, TOut> input, int min = 0,
                                                               int max = int.MaxValue)
        {
            return input.Many(min, max,
            () => new List<TOut>().As<IList<TOut>>(),
            (prev, next) =>
            {
                prev.Add(next);
                return prev;
            });
        }

        public static Parser<TIn, TOut> FollowedBy<TIn,TOut>(this Parser<TIn, TOut> previous, Parser<TIn> next)
        {
            return new FollowedBy<TIn,TOut>(previous, next);
        }

        public static Parser<T, TOut> Fail<T, TOut>(string message)
        {
            return new Fail<T, TOut>(message);
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

        public static Parser<T, T> Satisfy<T>(Expression<Func<T, bool>> predicate)
        {
            return new Satisfy<T>(predicate);
        }

        public static Parser<TIn> EndOfInput<TIn>()
        {
            return Combinators.EndOfInput<TIn>.Instance;
        }

        public static Parser<TIn> Many<TIn>(this Parser<TIn> parser, int min = 0, int max = int.MaxValue)
        {
            return Combinators.Many<TIn>.Create(parser, min, max);
        }
        
        public static Parser<TIn, IList<TOut>> SepBy<TIn, TOut>(this Parser<TIn, TOut> parser, Parser<TIn> separator, int min = 0, int max = int.MaxValue)
        {
            return new SepBy<TIn, TOut>(parser, separator, min, max);
        }

        public static Parser<TIn> SepBy<TIn>(this Parser<TIn> parser, Parser<TIn> separator, int min = 0,int max = int.MaxValue)
        {
            return new SepBy<TIn>(parser, separator, min, max);
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

        public static Deferred<TIn,TOut> Deferred<TIn, TOut>()
        {
            return new Deferred<TIn, TOut>();
        }

        public static Deferred<TIn> Deferred<TIn>()
        {
            return new Deferred<TIn>();
        }

        public static Parser<TIn> Optional<TIn>(this Parser<TIn> parser)
        {
            return parser.Many(0, 1);
        }

        public static Parser<TIn> Name<TIn>(this Parser<TIn> parser, string name)
        {
            return new Named<TIn>(parser, name);
        }

        public static Parser<TIn, TOut> Name<TIn, TOut>(this Parser<TIn, TOut> parser, string name)
        {
            return new Named<TIn, TOut>(parser, name);
        }

        public static Parser<TIn> SkipWhile<TIn>(Func<TIn, bool> predicate, int min = 0, int max = int.MaxValue)
        {
            return new SkipWhile<TIn>(predicate, min, max);
        }

        public static Parser<TIn, IList<TIn>> TakeWhile<TIn>(Func<TIn, bool> predicate, int min = 0, int max = int.MaxValue)
        {
            return new TakeWhile<TIn>(predicate, min, max);
        }
    }
}
