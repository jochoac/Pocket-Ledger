using LanguageExt;
using Microsoft.EntityFrameworkCore;
using PocketLedger.Core.SourceEvents.Models;
using PocketLedger.Core.SourceEvents.Ports;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.GuidTypes;
using PocketLedger.Domain.Entities;
using PocketLedger.Infrastructure.Persistence.Mappers;
using PocketLedger.Infrastructure.Persistence.Extensions;
using static LanguageExt.Prelude;

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

    public async Task<Validation<Error, SourceEvent>> GetById(SourceEventId id, CancellationToken ct)
    {
        try
        {
            var oSourceEvent = Optional(await dbContext.SourceEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(evt => evt.Id == (Guid)id, ct));

            return oSourceEvent
                .Map(SourceEventMapper.ToDomain)
                .ToValidation<Error>(new EntityNotFoundError(nameof(SourceEvent), id.ToString()));
        }
        catch (Exception ex)
        {
            return Errors.ExceptionError<SourceEvent>($"Unexpected error while fetching source event. {ex.Message}", ex);
        }
    }

    public async Task<Validation<Error, Seq<SourceEvent>>> List(SourceEventFilters filters, CancellationToken ct)
    {
        try
        {
            var query = dbContext.SourceEvents
                .AsNoTracking()
                .Apply(filters)
                .OrderByDescending(evt => evt.ReceivedAt);

            var entities = await query.ToListAsync(ct);

            return toSeq(entities.Map(SourceEventMapper.ToDomain));
        }
        catch (Exception ex)
        {
            return Errors.ExceptionError<Seq<SourceEvent>>(
                $"Unexpected error while listing source events. {ex.Message}",
                ex);
        }
    }
}