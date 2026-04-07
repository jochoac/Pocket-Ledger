namespace PocketLedger.Domain.Common.Primitives.GuidTypes;

public readonly record struct SourceEventId(Guid Value) : IGuidValue
{
    public object RawValue => Value;

    public static SourceEventId New() => new(Guid.NewGuid());
    public static SourceEventId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(SourceEventId value) => value.Value;
    public static implicit operator SourceEventId(Guid value) => new(value);
}