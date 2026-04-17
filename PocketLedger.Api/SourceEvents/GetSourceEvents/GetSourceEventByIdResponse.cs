namespace PocketLedger.Api.SourceEvents.GetSourceEvents;

public sealed record GetSourceEventByIdResponse(
    Guid SourceEventId,
    string SourceType,
    string RawPayload,
    DateTimeOffset ReceivedAt,
    string ExternalId);