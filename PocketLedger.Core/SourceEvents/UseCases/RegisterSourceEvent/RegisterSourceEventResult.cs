using PocketLedger.Domain.Common.Primitives.GuidTypes;

namespace PocketLedger.Core.SourceEvents.UseCases.RegisterSourceEvent;

public sealed record RegisterSourceEventResult(SourceEventId SourceEventId);