using LanguageExt;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;
using PocketLedger.Domain.Entities;
using static PocketLedger.Domain.Common.Prelude;

namespace PocketLedger.Core.UnitTests.SourceEvents.RegisterSourceEvent;

//TODO: Centralize
public static class SourceEventHelpers
{
    public static SourceEvent BuildSourceEvent(
        Option<SourceEventType> oType = default,
        Option<RawPayload> oPayload = default,
        Option<DateTimeOffset> oReceivedAt = default,
        Option<DateTimeOffset> oOccurredAt = default,
        Option<ExternalId> oExternalId = default) =>
        SourceEvent.Create(
            oType.IfNone(BuildSourceType),
            oPayload.IfNone(BuildRawPayload),
            oReceivedAt.IfNone(() => DateTimeOffset.UtcNow),
            oOccurredAt,
            oExternalId);

    public static SourceEventType BuildSourceType() => SourceEventType.Wallet;

    public static RawPayload BuildRawPayload() =>
        RawPayload("""
                   {
                     "source": "Wallet",
                     "message": "Compra aprobada por 23500 COP en OXXO",
                     "merchant": "OXXO",
                     "amount": 23500,
                     "currency": "COP"
                   }
                   """);
}