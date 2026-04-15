namespace PocketLedger.Domain.Common.Primitives.StringTypes;

public readonly record struct ErrorMessage(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(ErrorMessage value) => value.Value;
    public static implicit operator ErrorMessage(string value) => new(value);
}