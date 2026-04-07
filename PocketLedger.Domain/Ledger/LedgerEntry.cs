using LanguageExt;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives;

namespace PocketLedger.Domain.Ledger;

using static LanguageExt.Prelude;

public sealed record LedgerEntry(
    LedgerEntryId Id,
    AccountId AccountId,
    Option<CategoryId> CategoryId,
    Option<SourceEventId> SourceEventId,
    TransactionType TransactionType,
    Money Amount,
    CurrencyCode CurrencyCode,
    EntryDescription Description,
    Option<MerchantName> MerchantName,
    Option<ExternalReference> ExternalReference,
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
        CurrencyCode currencyCode,
        EntryDescription description,
        Option<MerchantName> merchantName,
        Option<ExternalReference> externalReference,
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
                externalReference,
                occurredAt,
                createdAt);
    }
}