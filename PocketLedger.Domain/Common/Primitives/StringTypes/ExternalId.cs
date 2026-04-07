namespace PocketLedger.Domain.Common.Primitives.StringTypes;

public readonly record struct ExternalId(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(ExternalId value) => value.Value;
    public static implicit operator ExternalId(string value) => new(value);
}