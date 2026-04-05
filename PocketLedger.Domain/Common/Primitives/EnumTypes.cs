namespace PocketLedger.Domain.Common.Primitives;

public enum TransactionTypeValue
{
    Expense = 1,
    Income = 2,
    Transfer = 3,
    Adjustment = 4
}

public readonly record struct TransactionType(TransactionTypeValue Value) : IPrimitiveValue<TransactionTypeValue>
{
    public object RawValue => Value;

    public static TransactionType Expense => new(TransactionTypeValue.Expense);
    public static TransactionType Income => new(TransactionTypeValue.Income);
    public static TransactionType Transfer => new(TransactionTypeValue.Transfer);
    public static TransactionType Adjustment => new(TransactionTypeValue.Adjustment);

    public override string ToString() => Value.ToString();

    public static implicit operator TransactionTypeValue(TransactionType value) => value.Value;
    public static implicit operator TransactionType(TransactionTypeValue value) => new(value);
}