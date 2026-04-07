using Prelude = LanguageExt.Prelude;

namespace PocketLedger.Domain.Entities;

using LanguageExt;
using Common.ErrorTypes;
using Common.Primitives.EnumTypes;
using Common.Primitives.GuidTypes;
using Common.Primitives.NumericTypes;
using Common.Primitives.StringTypes;
using static Prelude;

public sealed record LedgerEntry(
    LedgerEntryId Id,
    AccountId AccountId,
    Option<CategoryId> CategoryId,
    Option<SourceEventId> SourceEventId,
    TransactionType TransactionType,
    Money Amount,
    Currency CurrencyCode,
    Description Description,
    Option<Name> MerchantName,
    Option<ExternalId> ExternalId,
    DateTimeOffset OccurredAt,
    DateTimeOffset CreatedAt)
{
    public static Validation<Error, LedgerEntry> Create(
        LedgerEntryId id,
        AccountId accountId,
        Option<CategoryId> categoryId,
        Option<SourceEventId> sourceEventId,
        TransactionType transactionType,
        Money amount,
        Currency currencyCode,
        Description description,
        Option<Name> merchantName,
        Option<ExternalId> externalId,
        DateTimeOffset occurredAt,
        DateTimeOffset createdAt)
    {
        var categoryRules =
            transactionType.Value switch
            {
                TransactionTypeValue.Expense when categoryId.IsNone =>
                    Errors.InvalidValue<Unit>(
                        nameof(CategoryId),
                        "Expense transactions require a category."),

                TransactionTypeValue.Transfer when categoryId.IsSome =>
                    Errors.InvalidValue<Unit>(
                        nameof(CategoryId),
                        "Transfer transactions must not have a category."),

                _ => Success<Error, Unit>(unit)
            };

        var dateRules =
            occurredAt > createdAt
                ? Errors.InvalidValue<Unit>(
                    nameof(OccurredAt),
                    "OccurredAt cannot be after CreatedAt.")
                : Success<Error, Unit>(unit);

        return
            from _1 in categoryRules
            from _2 in dateRules
            select new LedgerEntry(
                id,
                accountId,
                categoryId,
                sourceEventId,
                transactionType,
                amount,
                currencyCode,
                description,
                merchantName,
                externalId,
                occurredAt,
                createdAt);
    }
}