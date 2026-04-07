namespace PocketLedger.Domain.Common.Primitives.EnumTypes;

public enum AccountTypeValue
{
    Cash = 1,
    Checking = 2,
    Savings = 3,
    CreditCard = 4,
    DigitalWallet = 5
}

public readonly record struct AccountType(AccountTypeValue Value) : IPrimitiveValue<AccountTypeValue>
{
    public object RawValue => Value;

    public static AccountType Cash => new(AccountTypeValue.Cash);
    public static AccountType Checking => new(AccountTypeValue.Checking);
    public static AccountType Savings => new(AccountTypeValue.Savings);
    public static AccountType CreditCard => new(AccountTypeValue.CreditCard);
    public static AccountType DigitalWallet => new(AccountTypeValue.DigitalWallet);

    public override string ToString() => Value.ToString();

    public static implicit operator AccountTypeValue(AccountType value) => value.Value;
    public static implicit operator AccountType(AccountTypeValue value) => new(value);
}