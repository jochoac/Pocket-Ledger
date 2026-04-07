using LanguageExt;
using PocketLedger.Domain.Common.ErrorTypes;
using static LanguageExt.Prelude;

namespace PocketLedger.Domain.Common.Primitives.NumericTypes;

public readonly record struct Money(decimal Value) : IDecimalValue
{
    public object RawValue => Value;

    public override string ToString() => Value.ToString("0.##");

    public static implicit operator decimal(Money value) => value.Value;
    public static implicit operator Money(decimal value) => new(value);

    public static Validation<Error, Money> Create(decimal value) =>
        value <= 0
            ? Errors.InvalidValue<Money>(nameof(Money), "Amount must be greater than zero.")
            : Success<Error, Money>(new Money(value));
}