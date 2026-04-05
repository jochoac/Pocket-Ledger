using LanguageExt;

namespace PocketLedger.Domain.Common.ErrorTypes;

public abstract record Error
{
    public abstract string Message { get; }

    public override string ToString() => $"{GetType().Name} :: {Message}";

    public static AggregateException ToExceptions(Seq<Error> errors, string? message = null) =>
        new(message, errors.Map(error => new Exception(error.Message)));
}