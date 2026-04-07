namespace PocketLedger.Domain.Common.Primitives.GuidTypes;

public readonly record struct CategoryId(Guid Value) : IGuidValue
{
    public object RawValue => Value;

    public static CategoryId New() => new(Guid.NewGuid());
    public static CategoryId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(CategoryId value) => value.Value;
    public static implicit operator CategoryId(Guid value) => new(value);
}
