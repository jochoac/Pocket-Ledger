namespace PocketLedger.Domain.Entities;

using LanguageExt;
using Common.Primitives.EnumTypes;
using Common.Primitives.GuidTypes;
using Common.Primitives.StringTypes;

public sealed class SourceEvent
{
    public SourceEventId Id { get; }
    public SourceEventType Type { get; }
    public RawPayload RawPayload { get; }
    public DateTimeOffset ReceivedAt { get; }
    public Option<DateTimeOffset> OccurredAt { get; }
    public Option<ExternalId> ExternalId { get; }

    private SourceEvent(
        SourceEventId id,
        SourceEventType type,
        RawPayload rawPayload,
        DateTimeOffset receivedAt,
        Option<DateTimeOffset> occurredAt,
        Option<ExternalId> externalId)
    {
        Id = id;
        Type = type;
        RawPayload = rawPayload;
        ReceivedAt = receivedAt;
        OccurredAt = occurredAt;
        ExternalId = externalId;
    }

    public static SourceEvent Create(
        SourceEventType type,
        RawPayload rawPayload,
        DateTimeOffset receivedAt,
        Option<DateTimeOffset> occurredAt = default,
        Option<ExternalId> externalId = default)
        => new(
            SourceEventId.New(),
            type,
            rawPayload,
            receivedAt,
            occurredAt,
            externalId);

    public static SourceEvent Rehydrate(
        SourceEventId id,
        SourceEventType type,
        RawPayload rawPayload,
        DateTimeOffset receivedAt,
        Option<DateTimeOffset> occurredAt,
        Option<ExternalId> externalId)
        => new(
            id,
            type,
            rawPayload,
            receivedAt,
            occurredAt,
            externalId);
}