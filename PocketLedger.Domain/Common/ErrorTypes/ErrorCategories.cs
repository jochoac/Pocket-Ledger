namespace PocketLedger.Domain.Common.ErrorTypes;

public abstract record ValidationError(string InnerMessage) : Error
{
    public override string Message => $"Validation :: {InnerMessage}";
}

public abstract record NotFoundError(string InnerMessage) : Error
{
    public override string Message => $"NotFound :: {InnerMessage}";
}

public abstract record ConflictError(string InnerMessage) : Error
{
    public override string Message => $"Conflict :: {InnerMessage}";
}

public abstract record UnauthorizedError(string InnerMessage) : Error
{
    public override string Message => $"Unauthorized :: {InnerMessage}";
}

public abstract record ForbiddenError(string InnerMessage) : Error
{
    public override string Message => $"Forbidden :: {InnerMessage}";
}

public abstract record InternalError(string InnerMessage) : Error
{
    public override string Message => $"Internal :: {InnerMessage}";
}