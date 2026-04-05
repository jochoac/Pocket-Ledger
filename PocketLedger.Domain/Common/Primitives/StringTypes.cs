namespace PocketLedger.Domain.Common.Primitives;

public readonly record struct CurrencyCode(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(CurrencyCode value) => value.Value;
    public static implicit operator CurrencyCode(string value) => new(value);
}

public readonly record struct EntryDescription(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(EntryDescription value) => value.Value;
    public static implicit operator EntryDescription(string value) => new(value);
}

public readonly record struct MerchantName(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(MerchantName value) => value.Value;
    public static implicit operator MerchantName(string value) => new(value);
}

public readonly record struct ExternalReference(string Value) : IStringValue
{
    public object RawValue => Value;

    public override string ToString() => Value;

    public static implicit operator string(ExternalReference value) => value.Value;
    public static implicit operator ExternalReference(string value) => new(value);
}