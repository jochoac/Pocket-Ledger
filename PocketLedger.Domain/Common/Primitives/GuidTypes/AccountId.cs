namespace PocketLedger.Domain.Common.Primitives.GuidTypes;

public readonly record struct AccountId(Guid Value) : IGuidValue
{
    public object RawValue => Value;

    public static AccountId New() => new(Guid.NewGuid());
    public static AccountId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(AccountId value) => value.Value;
    public static implicit operator AccountId(Guid value) => new(value);
}