using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Entities;
using PocketLedger.Infrastructure.Persistence.Models;
using static LanguageExt.Prelude;
using static PocketLedger.Domain.Common.Prelude;

namespace PocketLedger.Infrastructure.Persistence.Mappers;

public static class SourceEventMapper
{
    public static SourceEventRecord ToRecord(SourceEvent sourceEvent)
    {
        var a = new SourceEventRecord
        {
            Id = sourceEvent.Id.Value,
            SourceType = (int)sourceEvent.Type.Value,
            RawPayload = sourceEvent.RawPayload.Value,
            ReceivedAt = sourceEvent.ReceivedAt.ToUniversalTime(),
            OccurredAt = sourceEvent.OccurredAt
                .Map(occurredAt => (DateTimeOffset?)occurredAt.ToUniversalTime())
                .IfNoneUnsafe(() => null),
            ExternalId = sourceEvent.ExternalId
                .Map(string? (id) => id.Value)
                .IfNoneUnsafe(() => null),
        };
        return a;
    }

    public static SourceEvent ToDomain(SourceEventRecord record) =>
        SourceEvent.Rehydrate(
            SourceEventId(record.Id),
            new SourceEventType((SourceEventTypeValue)record.SourceType),
            RawPayload(record.RawPayload),
            record.ReceivedAt,
            Optional(record.OccurredAt),
            Optional(record.ExternalId)
                .Filter(static x => !string.IsNullOrWhiteSpace(x))
                .Map(static x => ExternalId(x))
        );
}