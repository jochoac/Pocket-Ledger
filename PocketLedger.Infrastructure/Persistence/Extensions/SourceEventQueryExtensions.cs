using LanguageExt;
using PocketLedger.Core.SourceEvents.Models;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;
using PocketLedger.Infrastructure.Persistence.Models;

namespace PocketLedger.Infrastructure.Persistence.Extensions;

internal static class SourceEventQueryExtensions
{
    extension(IQueryable<SourceEventRecord> query)
    {
        public IQueryable<SourceEventRecord> Apply(SourceEventFilters filters) =>
            query
                .ApplySourceType(filters.SourceType)
                .ApplyExternalId(filters.ExternalId);

        private IQueryable<SourceEventRecord> ApplySourceType(Option<SourceEventType> oSourceType) =>
            oSourceType
                .Map(sourceType => query.Where(evt => evt.SourceType == (int)sourceType.Value))
                .IfNone(() => query);

        private IQueryable<SourceEventRecord> ApplyExternalId(Option<ExternalId> oExternalId) =>
            oExternalId
                .Map(externalId => query.Where(evt => evt.ExternalId == (string)externalId))
                .IfNone(() => query);
    }
}