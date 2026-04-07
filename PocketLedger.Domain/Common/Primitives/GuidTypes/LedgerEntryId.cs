namespace PocketLedger.Domain.Common.Primitives.GuidTypes;

public readonly record struct LedgerEntryId(Guid Value) : IGuidValue
{
    public object RawValue => Value;

    public static LedgerEntryId New() => new(Guid.NewGuid());
    public static LedgerEntryId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(LedgerEntryId value) => value.Value;
    public static implicit operator LedgerEntryId(Guid value) => new(value);
}