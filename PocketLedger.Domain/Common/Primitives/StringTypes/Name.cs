namespace PocketLedger.Domain.Common.Primitives.StringTypes;

public readonly record struct Name(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(Name value) => value.Value;
    public static implicit operator Name(string value) => new(value);
}