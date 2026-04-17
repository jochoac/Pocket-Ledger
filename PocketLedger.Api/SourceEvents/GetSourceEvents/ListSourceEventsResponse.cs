namespace PocketLedger.Api.SourceEvents.GetSourceEvents;

public sealed record ListSourceEventResponse(
    Guid SourceEventId,
    string SourceType,
    string RawPayload,
    DateTimeOffset ReceivedAt,
    string ExternalId);