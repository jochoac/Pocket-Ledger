using LanguageExt;
using PocketLedger.Domain.Common.ErrorTypes;
using static LanguageExt.Prelude;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.GuidTypes;
using PocketLedger.Domain.Common.Primitives.NumericTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;

namespace PocketLedger.Domain.Ledger;

public sealed record Account(
    AccountId Id,
    Name Name,
    AccountType Type,
    Currency Currency,
    Option<Money> InitialBalance,
    bool IsActive,
    DateTimeOffset CreatedAt,
    Option<DateTimeOffset> UpdatedAt)
{
    public static Validation<Error, Account> Create(
        AccountId id,
        Name name,
        AccountType type,
        Currency currency,
        Option<Money> initialBalance,
        bool isActive,
        DateTimeOffset createdAt,
        Option<DateTimeOffset> updatedAt) =>
        Success<Error, Account>(
            new Account(
                id,
                name,
                type,
                currency,
                initialBalance,
                isActive,
                createdAt,
                updatedAt));

    public Account Deactivate(DateTimeOffset updatedAt) =>
        this with
        {
            IsActive = false,
            UpdatedAt = Some(updatedAt)
        };

    public Account Activate(DateTimeOffset updatedAt) =>
        this with
        {
            IsActive = true,
            UpdatedAt = Some(updatedAt)
        };

    public Account UpdateName(Name name, DateTimeOffset updatedAt) =>
        this with
        {
            Name = name,
            UpdatedAt = Some(updatedAt)
        };
}