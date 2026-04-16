using Microsoft.AspNetCore.Mvc;
using PocketLedger.Api.Common.Http;
using PocketLedger.Core.SourceEvents.UseCases.GetSourceEvent;
using PocketLedger.Domain.Common.Primitives.GuidTypes;
using PocketLedger.Domain.Entities;

namespace PocketLedger.Api.SourceEvents.GetSourceEventById;

public static class GetSourceEventByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetSourceEventByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/source-events/{id:guid}", Handle)
            .WithName("GetSourceEventById")
            .WithSummary("Gets a source event by id")
            .Produces<GetSourceEventByIdResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static async Task<IResult> Handle(
        Guid id,
        GetSourceEventHandler handler,
        CancellationToken cancellationToken)
    {
        var vSourceEvent = await handler.GetSourceEventByIdAsync(new SourceEventId(id), cancellationToken);
        return vSourceEvent
            .Map(sourceEvent => Results.Ok(MapResponse(sourceEvent)))
            .IfFail(HttpErrorMapper.ToProblem);
    }

    private static GetSourceEventByIdResponse MapResponse(SourceEvent sourceEvent) =>
        new(
            sourceEvent.Id.Value,
            sourceEvent.Type.ToString().ToLowerInvariant(), 
            sourceEvent.RawPayload.Value,
            sourceEvent.ReceivedAt,
            sourceEvent.ExternalId.IfNone(string.Empty));
}