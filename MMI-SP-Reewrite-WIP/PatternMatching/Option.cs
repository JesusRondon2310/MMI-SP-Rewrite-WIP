using System;

namespace MMI_SP.PatternMatching
{
    public abstract class Option<T>
    {
        public bool IsSome => this is Some<T>;
        public bool IsNone => this is None<T>;
    }

    public sealed class Some<T> : Option<T>
    {
        public T Value { get; }
        public Some(T value) => Value = value;
    }

    public sealed class None<T> : Option<T> { }

    public static class Option
    {
        public static Option<T> FromNullable<T>(T value) where T : class
            => value != null ? new Some<T>(value) : (Option<T>)new None<T>();

        public static None<T> NewNone<T>() => new None<T>();
    }
}