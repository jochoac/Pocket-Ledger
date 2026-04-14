namespace PocketLedger.Domain.Common.ErrorTypes;

public sealed record RequiredValueError(string FieldName)
    : ValidationError($"{FieldName} is required.");

public sealed record EmptyStringError(string FieldName)
    : ValidationError($"{FieldName} cannot be empty.");

public sealed record InvalidValueError(string FieldName, string Reason)
    : ValidationError($"{FieldName} is invalid. {Reason}");

public sealed record InvalidEnumValueError(string FieldName, string Value)
    : ValidationError($"{FieldName} has invalid value '{Value}'.");

public sealed record ValueOutOfRangeError(string FieldName, string Reason)
    : ValidationError($"{FieldName} is out of range. {Reason}");

public sealed record EntityNotFoundError(string EntityName, string Id)
    : NotFoundError($"{EntityName} with id '{Id}' was not found.");

public sealed record DuplicateEntityError(string EntityName, string Value)
    : ConflictError($"{EntityName} with value '{Value}' already exists.");

public sealed record UnexpectedError(string Reason)
    : InternalError(Reason);
    
public sealed record ExceptionError(string Reason, Exception Exception)
    : InternalError($"An exception occurred. {Reason} Exception: {Exception.Message}");