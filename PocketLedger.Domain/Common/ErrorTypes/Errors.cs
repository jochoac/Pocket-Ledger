using LanguageExt;
using static LanguageExt.Prelude;

namespace PocketLedger.Domain.Common.ErrorTypes;

public static class Errors
{
    public static Validation<Error, T> Validation<T>(Error error) =>
        Fail<Error, T>(Seq1(error));

    public static Validation<Error, T> Validation<T>(Seq<Error> errors) =>
        Fail<Error, T>(errors);

    public static Validation<Error, T> Success<T>(T value) =>
       Success<Error, T>(value);

    public static Either<Error, T> Left<T>(Error error) =>
        Left<Error, T>(error);

    public static Either<Error, T> Right<T>(T value) =>
        Right<Error, T>(value);

    public static Validation<Error, T> Required<T>(string fieldName) =>
        Validation<T>(new RequiredValueError(fieldName));

    public static Validation<Error, T> EmptyString<T>(string fieldName) =>
        Validation<T>(new EmptyStringError(fieldName));

    public static Validation<Error, T> InvalidValue<T>(string fieldName, string reason) =>
        Validation<T>(new InvalidValueError(fieldName, reason));

    public static Validation<Error, T> InvalidEnumValue<T>(string fieldName, string value) =>
        Validation<T>(new InvalidEnumValueError(fieldName, value));

    public static Validation<Error, T> OutOfRange<T>(string fieldName, string reason) =>
        Validation<T>(new ValueOutOfRangeError(fieldName, reason));

    public static Validation<Error, T> NotFound<T>(string entityName, string id) =>
        Validation<T>(new EntityNotFoundError(entityName, id));

    public static Validation<Error, T> Conflict<T>(string entityName, string value) =>
        Validation<T>(new DuplicateEntityError(entityName, value));

    public static Validation<Error, T> Internal<T>(string reason) =>
        Validation<T>(new UnexpectedError(reason));

    public static Validation<Error, T> ExceptionError<T>(string reason, Exception ex) =>
        Validation<T>(new ExceptionError(reason, ex));
}