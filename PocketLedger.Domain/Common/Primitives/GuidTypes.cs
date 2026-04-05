namespace PocketLedger.Domain.Common.Primitives;

public readonly record struct LedgerEntryId(Guid Value) : IGuidValue
{
    public object RawValue => Value;

    public static LedgerEntryId New() => new(Guid.NewGuid());
    public static LedgerEntryId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(LedgerEntryId value) => value.Value;
    public static implicit operator LedgerEntryId(Guid value) => new(value);
}

public readonly record struct AccountId(Guid Value) : IGuidValue
{
    public object RawValue => Value;

    public static AccountId New() => new(Guid.NewGuid());
    public static AccountId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(AccountId value) => value.Value;
    public static implicit operator AccountId(Guid value) => new(value);
}

public readonly record struct CategoryId(Guid Value) : IGuidValue
{
    public object RawValue => Value;

    public static CategoryId New() => new(Guid.NewGuid());
    public static CategoryId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(CategoryId value) => value.Value;
    public static implicit operator CategoryId(Guid value) => new(value);
}

public readonly record struct SourceEventId(Guid Value) : IGuidValue
{
    public object RawValue => Value;

    public static SourceEventId New() => new(Guid.NewGuid());
    public static SourceEventId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(SourceEventId value) => value.Value;
    public static implicit operator SourceEventId(Guid value) => new(value);
}