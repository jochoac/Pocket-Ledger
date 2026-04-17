using LanguageExt;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.GuidTypes;
using PocketLedger.Domain.Common.Primitives.NumericTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;
using static LanguageExt.Prelude;

namespace PocketLedger.Domain.Common;

public static class Prelude
{
    //Guid
    public static AccountId AccountId(Guid value) => new(value);
    public static SourceEventId SourceEventId(Guid value) => new(value);
    public static CategoryId CategoryId(Guid value) => new(value);
    public static LedgerEntryId LedgerEntryId(Guid value) => new(value);

    //String
    public static Description Description(string value) => new(value);
    public static ErrorMessage ErrorMessage(string value) => new(value);
    public static Name Name(string value) => new(value);
    public static RawPayload RawPayload(string value) => new(value);
    public static ExternalId ExternalId(string value) => new(value);

    //Numeric
    public static Money Money(decimal amount) => new(amount);

    //Enum
    public static Currency Currency(CurrencyValue value) => new(value);
    public static AccountType AccountType(AccountType value) => new(value);
    public static TransactionType TransactionType(TransactionType value) => new(value);

    //Special
    public static Validation<Error, Successful> Successfully =>
        Success<Error, Successful>(new Successful());
}

public readonly struct Successful
{
    public override string ToString() => "Prelude.Successful";

    // public Func<T, Successful> ly<T>() => o => Prelude.Successful;
    public static Successful ly<T>(T t) => new();
    public static implicit operator Func<Successful>(Successful s) => () => s;
    public static implicit operator Func<object, Successful>(Successful s) => _ => s;
}