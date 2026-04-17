using LanguageExt;
using PocketLedger.Core.SourceEvents.Models;
using PocketLedger.Core.SourceEvents.Ports;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.GuidTypes;
using PocketLedger.Domain.Entities;

namespace PocketLedger.Core.SourceEvents.UseCases.GetSourceEvent;

public class GetSourceEventHandler(ISourceEventRepository sourceEventRepository)
{
    public Task<Validation<Error, SourceEvent>> GetSourceEventByIdAsync(SourceEventId id, CancellationToken cancellationToken) =>
        sourceEventRepository.GetById(id, cancellationToken);

    public Task<Validation<Error, Seq<SourceEvent>>> ListSourceEvents(SourceEventFilters filters, CancellationToken ct) =>
        sourceEventRepository.List(filters, ct);
}