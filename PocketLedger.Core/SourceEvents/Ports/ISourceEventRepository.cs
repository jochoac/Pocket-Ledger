using LanguageExt;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Entities;

namespace PocketLedger.Core.SourceEvents.Ports;

public interface ISourceEventRepository
{
    Task<Validation<Error, SourceEvent>> Add(SourceEvent sourceEvent, CancellationToken cancellationToken);
}