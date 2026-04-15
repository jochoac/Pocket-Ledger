namespace PocketLedger.Api.SourceEvents.RegisterSourceEvent;

public sealed record RegisterSourceEventRequest(
    string RawPayload,
    DateTimeOffset ReceivedAt,
    string? ExternalId);