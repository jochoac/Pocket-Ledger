namespace PocketLedger.Infrastructure.Persistence.Models;

public sealed class SourceEventRecord
{
    public Guid Id { get; set; }

    public int SourceType { get; set; }

    public string RawPayload { get; set; } = string.Empty;

    public DateTimeOffset ReceivedAt { get; set; }

    public DateTimeOffset? OccurredAt { get; set; }

    public string? ExternalId { get; set; }
}