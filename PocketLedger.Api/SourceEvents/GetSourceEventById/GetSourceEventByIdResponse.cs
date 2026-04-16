namespace PocketLedger.Api.SourceEvents.GetSourceEventById;

public sealed record GetSourceEventByIdResponse(
    Guid SourceEventId,
    string SourceType,
    string RawPayload,
    DateTimeOffset ReceivedAt,
    string ExternalId);