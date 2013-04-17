using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Apparser.Input;
using Outcomes;
using Yagul.Types;

namespace Apparser.Parser.Combinators
{
    class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }
    }

    internal sealed class Satisfy<T> : Parser<T, T>, IEquatable<Satisfy<T>>
    {
        private readonly Expression<Func<T, bool>> _predicate;
        private readonly Func<T, bool> _compiled; 
        
        public Satisfy(Expression<Func<T, bool>> predicate)
        {
            _predicate = predicate;
            _compiled = _predicate.Compile();
        }


        public override Result<string, Unit> Run<TSave>(IParserInput<T, TSave> input)
        {
            if (!input.MoveNext())
                return new Failure<string, Unit>("Unexpected end of input.");

            if (_compiled(input.Current))
                return new Outcomes.Success<string, Unit>(default(Unit));

            return new Failure<string, Unit>("Could not satisfy predicate.");
        }

        public override Result<string, T> RunWithResult<TSave>(IParserInput<T,TSave> input)
        {
            if (!input.MoveNext())
                return new Failure<string, T>("Unexpected end of input.");

            if (_compiled(input.Current))
                return new Outcomes.Success<string, T>(input.Current);

            return new Failure<string, T>("Could not satisfy predicate.");
        }

        public override Parser<T, TAgg> Many<TAgg>(int min, int max, Func<TAgg> agg, Func<TAgg, T, TAgg> aggregation)
        {
            //TODO: TakeWhile
            return base.Many<TAgg>(min, max, agg, aggregation);
        }
        
        public override bool Equals(Parser<T> other)
        {
            return Equals(other as Satisfy<T>);
        }

        public bool Equals(Satisfy<T> other)
        {
            return other != null &&
                   Equals(_predicate, other._predicate);
        }

        public override Parser<T, T> OrElse(Parser<T, T> other)
        {
            var satisfy = other as Satisfy<T>;
            if (satisfy != null)
            {
                var newBody = Expression.OrElse(_predicate.Body, new ParameterReplacer(_predicate.Parameters.First()).Visit(satisfy._predicate.Body));
                var lambda = (Expression<Func<T,bool>>)Expression.Lambda(newBody, _predicate.Parameters);
                return new Satisfy<T>(lambda);
            }
            return base.OrElse(other);
        }

        public override Parser<T> OrElse(Parser<T> other)
        {
            var satisfy = other as Satisfy<T>;
            if (satisfy != null)
            {
                var newBody = Expression.OrElse(_predicate.Body, new ParameterReplacer(_predicate.Parameters.First()).Visit(satisfy._predicate.Body));
                var lambda = (Expression<Func<T, bool>>)Expression.Lambda(newBody, _predicate.Parameters);
                return new Satisfy<T>(lambda);
            }
            return base.OrElse(other);
        }

        public override string Name
        {
            get { return "satisfying predicate"; }
        }

        public override bool CanMatchWithoutConsumingInput
        {
            get { return false; }
        }
    }
}