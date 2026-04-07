namespace PocketLedger.Domain.Common.Primitives.StringTypes;

public readonly record struct RawPayload(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(RawPayload value) => value.Value;
    public static implicit operator RawPayload(string value) => new(value);
}