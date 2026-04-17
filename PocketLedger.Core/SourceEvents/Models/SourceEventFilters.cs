using LanguageExt;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;

namespace PocketLedger.Core.SourceEvents.Models;

public readonly record struct SourceEventFilters(
    Option<SourceEventType> SourceType,
    Option<ExternalId> ExternalId)
{
    public static SourceEventFilters Empty =>
        new(
            SourceType: Option<SourceEventType>.None,
            ExternalId: Option<ExternalId>.None);
}