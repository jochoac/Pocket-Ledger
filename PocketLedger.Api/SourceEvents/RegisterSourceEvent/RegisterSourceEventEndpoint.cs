using LanguageExt;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PocketLedger.Core.SourceEvents.UseCases.RegisterSourceEvent;
using PocketLedger.Domain.Common.LanguageExt;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using static LanguageExt.Prelude;
using static PocketLedger.Domain.Common.Prelude;

namespace PocketLedger.Api.SourceEvents.RegisterSourceEvent;

public static class RegisterSourceEventEndpoint
{
    public static IEndpointRouteBuilder MapRegisterSourceEventEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/source-events/{sourceType}", Handle)
            .WithName("RegisterSourceEvent")
            .WithSummary("Registers a raw source event")
            .Produces<RegisterSourceEventResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static async Task<Results<
            Created<RegisterSourceEventResponse>,
            BadRequest<ProblemDetails>,
            ProblemHttpResult>>
        Handle(
            string sourceType,
            RegisterSourceEventRequest request,
            RegisterSourceEventHandler handler,
            CancellationToken cancellationToken) =>
        await TryParseSourceType(sourceType)
            .Map(async parsedSourceType =>
            {
                var command = new RegisterSourceEventCommand(
                    parsedSourceType,
                    RawPayload(request.RawPayload),
                    request.ReceivedAt,
                    Optional(request.ExternalId).Map(static x => ExternalId(x)));

                var result = await handler.Handle(command, cancellationToken);

                return result
                    .Map(success =>
                    {
                        var response = new RegisterSourceEventResponse(success.SourceEventId);
                        return (Results<
                            Created<RegisterSourceEventResponse>,
                            BadRequest<ProblemDetails>,
                            ProblemHttpResult>)TypedResults.Created(
                            $"/source-events/{response.SourceEventId}",
                            response);
                    })
                    .IfFail(errors =>
                        TypedResults.BadRequest(new ProblemDetails
                        {
                            Title = "Invalid source event request",
                            Detail = errors.Combine(),
                            Status = StatusCodes.Status400BadRequest
                        }));
            })
            .IfNone(() =>
                Task.FromResult<
                    Results<
                        Created<RegisterSourceEventResponse>,
                        BadRequest<ProblemDetails>,
                        ProblemHttpResult>>(
                    TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Invalid source type",
                        Detail = $"Unsupported source type '{sourceType}'.",
                        Status = StatusCodes.Status400BadRequest
                    })));

    private static Option<SourceEventType> TryParseSourceType(string sourceType) =>
        Enum.TryParse<SourceEventTypeValue>(sourceType, ignoreCase: true, out var parsed)
            ? new SourceEventType(parsed)
            : None;
}