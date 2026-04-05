using LanguageExt;
using PocketLedger.Domain.Common.Primitives;

namespace PocketLedger.Domain.Ledger;

public sealed record LedgerEntry(
    LedgerEntryId Id,
    AccountId AccountId,
    Option<CategoryId> CategoryId,
    Option<SourceEventId> SourceEventId,
    TransactionType TransactionType,
    Money Amount,
    CurrencyCode Currency,
    EntryDescription Description,
    Option<MerchantName> MerchantName,
    Option<ExternalReference> ExternalReference,
    DateTimeOffset OccurredAt,
    DateTimeOffset CreatedAt
);

