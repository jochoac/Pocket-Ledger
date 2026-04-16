using LanguageExt;
using PocketLedger.Core.SourceEvents.Ports;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.GuidTypes;
using PocketLedger.Domain.Entities;

namespace PocketLedger.Core.SourceEvents.UseCases.GetSourceEvent;

public class GetSourceEventHandler(ISourceEventRepository sourceEventRepository)
{
    public async Task<Validation<Error, SourceEvent>> GetSourceEventByIdAsync(SourceEventId id, CancellationToken cancellationToken) =>
        await sourceEventRepository.GetById(id, cancellationToken);
}