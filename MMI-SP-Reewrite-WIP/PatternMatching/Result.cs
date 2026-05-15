using System;

namespace MMI_SP.PatternMatching
{
    public abstract class Result<T>
    {
        public bool IsOk => this is Ok<T>;
        public bool IsErr => this is Err<T>;

        public abstract TResult Match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr);
    }

    public sealed class Ok<T> : Result<T>
    {
        public T Value { get; }
        public Ok(T value) => Value = value;

        public override TResult Match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr) => onOk(Value);
    }

    public sealed class Err<T> : Result<T>
    {
        public string Message { get; }
        public Err(string message) => Message = message;

        public override TResult Match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr) => onErr(Message);
    }
}