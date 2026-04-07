namespace PocketLedger.Domain.Common.Primitives.StringTypes;

public readonly record struct Description(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(Description value) => value.Value;
    public static implicit operator Description(string value) => new(value);
}