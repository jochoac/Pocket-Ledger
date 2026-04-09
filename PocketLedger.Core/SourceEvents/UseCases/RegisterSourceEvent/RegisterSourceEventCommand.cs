using LanguageExt;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;

namespace PocketLedger.Core.SourceEvents.UseCases.RegisterSourceEvent;

public sealed record RegisterSourceEventCommand(
    SourceEventType SourceType,
    RawPayload RawPayload,
    DateTimeOffset ReceivedAt,
    Option<ExternalId> ExternalId = default //Option<ExternalId>.None
);