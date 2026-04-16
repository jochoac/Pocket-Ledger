using LanguageExt;
using PocketLedger.Domain.Common.ErrorTypes;

namespace PocketLedger.Api.Common.Http;

public static class HttpErrorMapper
{
    public static IResult ToProblem(Seq<Error> errors)
    {
        if (errors.IsEmpty)
        {
            return Results.Problem(
                title: "Internal server error",
                detail: "Internal :: Unknown error.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var firstError = errors.Head();

        var (title, statusCode) = firstError switch
        {
            ValidationError => ("Validation error", StatusCodes.Status400BadRequest),
            NotFoundError => ("Resource not found", StatusCodes.Status404NotFound),
            ConflictError => ("Conflict", StatusCodes.Status409Conflict),
            UnauthorizedError => ("Unauthorized", StatusCodes.Status401Unauthorized),
            ForbiddenError => ("Forbidden", StatusCodes.Status403Forbidden),
            _ => ("Internal server error", StatusCodes.Status500InternalServerError)
        };

        return Results.Problem(
            title: title,
            detail: firstError.Message,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = errors.Map(error => error.Message).ToArray()
            });
    }
}