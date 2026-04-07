using LanguageExt;

namespace PocketLedger.Domain.Common.Primitives.EnumTypes;

public enum CurrencyValue
{
    Cop = 1,
    Usd = 2
}

public readonly record struct Currency(CurrencyValue Value) : IPrimitiveValue<CurrencyValue>
{
    public object RawValue => Value;

    public static Currency Cop => new(CurrencyValue.Cop);
    public static Currency Usd => new(CurrencyValue.Usd);

    public override string ToString() => ToCode();

    public string ToCode() =>
        Value switch
        {
            CurrencyValue.Cop => "COP",
            CurrencyValue.Usd => "USD",
            _ => throw new ArgumentOutOfRangeException(nameof(Value), Value, "Unsupported currency.")
        };

    public static Option<Currency> TryFromCode(string code) =>
        code.Trim().ToUpperInvariant() switch
        {
            "COP" => Cop,
            "USD" => Usd,
            _ => Option<Currency>.None
        };

    public static implicit operator CurrencyValue(Currency value) => value.Value;
    public static implicit operator Currency(CurrencyValue value) => new(value);
}