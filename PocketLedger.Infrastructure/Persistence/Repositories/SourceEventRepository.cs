using LanguageExt;
using Microsoft.EntityFrameworkCore;
using PocketLedger.Core.SourceEvents.Ports;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Entities;
using PocketLedger.Infrastructure.Persistence.Mappers;

namespace PocketLedger.Infrastructure.Persistence.Repositories;

public sealed class SourceEventRepository(PocketLedgerDbContext dbContext) : ISourceEventRepository
{
    public async Task<Validation<Error, SourceEvent>> Add(
        SourceEvent sourceEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            var record = SourceEventMapper.ToRecord(sourceEvent);
            await dbContext.SourceEvents.AddAsync(record, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return sourceEvent;
        }
        catch (DbUpdateException dbEx)
        {
            return Errors.ExceptionError<SourceEvent>($"Could not persist source event. {dbEx.Message}", dbEx);
        }
        catch (Exception ex)
        {
            return Errors.ExceptionError<SourceEvent>($"Unexpected error while persisting source event. {ex.Message}", ex);
        }
    }
}