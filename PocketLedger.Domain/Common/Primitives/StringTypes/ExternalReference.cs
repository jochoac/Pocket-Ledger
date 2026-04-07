namespace PocketLedger.Domain.Common.Primitives.StringTypes;

public readonly record struct ExternalReference(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(ExternalReference value) => value.Value;
    public static implicit operator ExternalReference(string value) => new(value);
}