using System;
using Yagul.Types;

namespace Outcomes
{
    /// <summary>
    /// A type that can represent failure or success, with a specific type for each.
    /// </summary>
    /// <typeparam name="TFail">The type of failed computations.</typeparam>
    /// <typeparam name="TSuccess">The type of successful computations.</typeparam>
    public abstract class Result<TFail, TSuccess>
    {
        internal Result()
        {}

        public static implicit operator Result<TFail, TSuccess>(Result<Any, TSuccess> any)
        {
            var failed = (Success<Any, TSuccess>)any;

            return new Success<TFail, TSuccess>(failed.Value);
        }

        public static implicit operator Result<TFail, TSuccess>(Result<TFail, Any> any)
        {
            var failed = (Failure<TFail, Any>)any;

            return new Failure<TFail, TSuccess>(failed.Value);
        }

        public static implicit operator Result<TFail, TSuccess>(TFail value)
        {
            return new Failure<TFail, TSuccess>(value);
        }

        public static implicit operator Result<TFail, TSuccess>(TSuccess value)
        {
            return new Success<TFail, TSuccess>(value);
        }

        public abstract bool IsSuccess { get; }
        public abstract bool IsFailure { get; }
        
        public abstract bool TryGetSuccess(out TSuccess success);
        public abstract bool TryGetFailure(out TFail failure);

        public abstract Result<TFail, TNewSuccess> SelectSuccess<TNewSuccess>(Func<TSuccess, TNewSuccess> projection);
        public abstract Result<TNewFailure, TSuccess> SelectFailure<TNewFailure>(Func<TFail, TNewFailure> projection);

        public abstract Result<TFail, TNewSuccess> SelectManySuccess<TNewSuccess>(Func<TSuccess, Result<TFail, TNewSuccess>> projection);
        public abstract Result<TNewFailure, TSuccess> SelectManyFailure<TNewFailure>(Func<TFail, Result<TNewFailure, TSuccess>> projection);
    }

    public static class Result
    {
        public static Result<TFail, Any> Failure<TFail>(TFail value)
        {
            return new Failure<TFail, Any>(value);
        }

        public static Result<Any, TSuccess> Success<TSuccess>(TSuccess value)
        {
            return new Success<Any, TSuccess>(value);
        }

        public static Result<Any, Unit> Success()
        {
            return new Success<Any, Unit>(Unit.It);
        }

        public static Result<TFail, TNewSuccess> Select<TFail, TSuccess, TNewSuccess>(this Result<TFail, TSuccess> me, Func<TSuccess, TNewSuccess> projection)
        {
            return me.SelectSuccess(projection);
        }

        public static Result<TFail, TNewSuccess> SelectMany<TFail, TSuccess, TNewSuccess>(this Result<TFail, TSuccess> me, Func<TSuccess, Result<TFail, TNewSuccess>> projection)
        {
            return me.SelectManySuccess(projection);
        }

        public static Result<TFail, TFinalSuccess> SelectMany<TFail, TSuccess, TNewSuccess, TFinalSuccess>(
            this Result<TFail, TSuccess> me, Func<TSuccess, Result<TFail, TNewSuccess>> projection,
            Func<TSuccess, TNewSuccess, TFinalSuccess> finalProjection)
        {
            return me.SelectManySuccess(x => projection(x).SelectSuccess(y => finalProjection(x, y)));
        }
    }

    public sealed class Success<TIgnored, T> : Result<TIgnored, T>
    {
        private readonly T _value;

        public Success(T message)
        {
            _value = message;
        }

        public T Value { get { return _value; } }

        public override bool IsSuccess
        {
            get { return true; }
        }

        public override bool IsFailure
        {
            get { return false; }
        }

        public override bool TryGetSuccess(out T success)
        {
            success = Value;
            return true;
        }

        public override bool TryGetFailure(out TIgnored failure)
        {
            failure = default(TIgnored);
            return false;
        }

        public override Result<TIgnored, TNewSuccess> SelectSuccess<TNewSuccess>(Func<T, TNewSuccess> projection)
        {
            return new Success<TIgnored,TNewSuccess>(projection(Value));
        }

        public override Result<TNewFailure, T> SelectFailure<TNewFailure>(Func<TIgnored, TNewFailure> projection)
        {
            return new Success<TNewFailure, T>(Value);
        }

        public override Result<TIgnored, TNewSuccess> SelectManySuccess<TNewSuccess>(Func<T, Result<TIgnored, TNewSuccess>> projection)
        {
            return projection(Value);
        }

        public override Result<TNewFailure, T> SelectManyFailure<TNewFailure>(Func<TIgnored, Result<TNewFailure, T>> projection)
        {
            return new Success<TNewFailure, T>(Value);
        }
    }

    public sealed class Failure<T, TIgnored> : Result<T, TIgnored>
    {
        private readonly T _value;

        public Failure(T value)
        {
            _value = value;
        }

        public T Value { get { return _value; } }

        public override bool IsSuccess
        {
            get { return false; }
        }

        public override bool IsFailure
        {
            get { return true; }
        }

        public override bool TryGetSuccess(out TIgnored success)
        {
            success = default(TIgnored);
            return false;
        }

        public override bool TryGetFailure(out T failure)
        {
            failure = Value;
            return true;
        }

        public override Result<T, TNewSuccess> SelectSuccess<TNewSuccess>(Func<TIgnored, TNewSuccess> projection)
        {
            return new Failure<T,TNewSuccess>(Value);
        }

        public override Result<TNewFailure, TIgnored> SelectFailure<TNewFailure>(Func<T, TNewFailure> projection)
        {
            return new Failure<TNewFailure, TIgnored>(projection(Value));
        }

        public override Result<T, TNewSuccess> SelectManySuccess<TNewSuccess>(Func<TIgnored, Result<T, TNewSuccess>> projection)
        {
            return new Failure<T,TNewSuccess>(Value);
        }

        public override Result<TNewFailure, TIgnored> SelectManyFailure<TNewFailure>(Func<T, Result<TNewFailure, TIgnored>> projection)
        {
            return projection(Value);
        }
    }
}//.mjngr4ewq