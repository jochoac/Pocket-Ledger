using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using PocketLedger.Api.Common.Http;
using PocketLedger.Core.SourceEvents.Models;
using PocketLedger.Core.SourceEvents.UseCases.GetSourceEvent;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Entities;
using static LanguageExt.Prelude;
using static PocketLedger.Domain.Common.Prelude;

namespace PocketLedger.Api.SourceEvents.GetSourceEvents;

public static class ListSourceEventsEndpoint
{
    public static IEndpointRouteBuilder MapListSourceEventsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/source-events", Handle)
            .WithName("ListSourceEvents")
            .WithSummary("Lists source events with optional filters")
            .Produces<List<ListSourceEventResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static async Task<IResult> Handle(
        [FromQuery] string? sourceType,
        [FromQuery] string? externalId,
        GetSourceEventHandler handler,
        CancellationToken cancellationToken) =>
        await BuildFilters(sourceType, externalId)
            .Map(async filters =>
            {
                var vSourceEvents = await handler.ListSourceEvents(filters, cancellationToken);

                return vSourceEvents
                    .Map(sourceEvents => Results.Ok(sourceEvents.Map(MapResponse).ToList()))
                    .IfFail(HttpErrorMapper.ToProblem);
            })
            .IfFail(errors => Task.FromResult(HttpErrorMapper.ToProblem(errors)));

    private static Validation<Error, SourceEventFilters> BuildFilters(
        string? sourceType,
        string? externalId)
    {
        var vSourceType = SourceEventType.Parse(sourceType ?? string.Empty);

        return vSourceType.Map(parsedSourceType =>
            new SourceEventFilters(
                SourceType: parsedSourceType,
                ExternalId: string.IsNullOrWhiteSpace(externalId)
                    ? None
                    : ExternalId(externalId)));
    }

    private static ListSourceEventResponse MapResponse(SourceEvent sourceEvent) =>
        new(
            sourceEvent.Id.Value,
            sourceEvent.Type.ToString().ToLowerInvariant(),
            sourceEvent.RawPayload.Value,
            sourceEvent.ReceivedAt,
            sourceEvent.ExternalId.IfNone(string.Empty));
}