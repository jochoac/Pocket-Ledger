using System.Diagnostics.Contracts;
using LanguageExt;
using LanguageExt.Common;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.LanguageExt.Extensions;
using PocketLedger.Domain.Common.Primitives.StringTypes;
using static LanguageExt.Prelude;
using Error = PocketLedger.Domain.Common.ErrorTypes.Error;

namespace PocketLedger.Domain.Common.LanguageExt
{
    public static class LanguageExtExtensions
    {
        public static Validation<Error, Successful> Successfully =>
            Success<Error, Successful>(new Successful());
        
        public static Validation<Error, T> Fail<T>(this Seq<Error> self) =>
            global::LanguageExt.Prelude.Fail<Error, T>(self);

        public static Validation<Error, T> ToValidation<T>
            (this Try<T> self, Func<Exception, Error> failFunc)
        {
            var optionalResult = self.Try();

            return optionalResult.Match(
                Success<Error, T>,
                ex => Fail<Error, T>(failFunc(ex))
            );
        }

        public static Validation<Error, T> ToValidation<T>
            (this TryOption<T> self, Func<Error> noneFunc, Func<Exception, Error> failFunc)
        {
            var optionalResult = self.Try();

            return optionalResult.Match(
                Success<Error, T>,
                () => Fail<Error, T>(noneFunc()),
                ex => Fail<Error, T>(failFunc(ex))
            );
        }

        public static Seq<T> ToSeq<T>(this T self) =>
            Seq1(self);

        public static Validation<Error, T> ToValidation<T>(this OptionalResult<T> self, Func<Option<Exception>, Error> makeError) =>
            self.Match(
                Success<Error, T>,
                () => Fail<Error, T>(makeError(None)),
                ex =>
                {
                    var createdError = makeError(Some(ex));
                    var exceptionError = new ExceptionError($"Exception in operation: {ex.Message}", ex);
                    return Seq(exceptionError, createdError);
                });
        
        
        public static Seq<T> OfType<T>(this Seq<Error> fs) where T : Error =>
            fs.Filter(f => f is T)
                .Map(f => (T)f);

        public static Seq<Error> NotOfType<T>(this Seq<Error> fs) where T : Error =>
            fs.Filter(f => !(f is T));

        public static ErrorMessage Combine(this Seq<ErrorMessage> errs)
        {
            var combined = errs.Map(f => f.Value).Combine("; ");
            return new ErrorMessage(combined);
        }

        public static string Combine<T>(this Seq<T> Errors) where T : Error
        {
            var combined = Errors.Map(f => f.ToString()).Combine("; ");
            return combined;
        }

        public static string Combine<T>(this IEnumerable<T> Errors) where T : Error
        {
            var combined = Errors.Map(f => f.ToString()).Combine("; ");
            return combined;
        }

        public static Option<T> ToOption<T>(this OptionalResult<T> input)
        {
            return input.Match(
                result => result,
                () => Option<T>.None,
                e => Option<T>.None
            );
        }

        public static HashMap<TK2, TV2> MapKVs<TK, TV, TK2, TV2>(this HashMap<TK, TV> hm, Func<(TK, TV), (TK2, TV2)> mapper) =>
            hm.Map(mapper).ToHashMap();

        //Useful for chaining functions
        public static TEnd Then<TStart, TEnd>(this TStart self, Func<TStart, TEnd> func) =>
            func(self);

        //An alias for chaining functions when the function changes the type
        public static TEnd As<TStart, TEnd>(this TStart self, Func<TStart, TEnd> func) => //This could restrict TEnd to be IParserrStruct
            Then(self, func);

        // Use languageExt Try to wrap a mapping function that might throw an exception
        public static OptionalResult<TOut> Try<TIn, TOut>(this TIn t, Func<TIn, TOut> func)
        {
            var a = TryOption(() => func(t));
            var b = a.Try();
            return b;
        }

        public static OptionalResult<TOut> Try<TIn, TOut>(this TIn t, Func<TIn, Option<TOut>> func)
        {
            var a = TryOption(() => func(t));
            var b = a.Try();
            return b;
        }

        public static Validation<Error, Successful> Try<TIn>(this TIn self, Action<TIn> func, Func<Exception, Error> Error)
        {
            try
            {
                func(self);
                return Successfully;
            }
            catch (Exception e)
            {
                return Fail<Error, Successful>(Error(e));
            }
        }

        public static Validation<Error, TOut> Try<TIn, TOut>(this TIn self, Func<TIn, TOut> func, Func<Exception, Error> Error)
        {
            try
            {
                var result = func(self);
                return Success<Error, TOut>(result);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        /// <summary>
        /// Just like Bind, but for OptionalResult instead of Option
        /// </summary>
        /// <param name="self"></param>
        /// <param name="func"></param>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <returns></returns>
        [Pure]
        public static OptionalResult<TB> Bind<TA, TB>(this OptionalResult<TA> self, Func<TA, OptionalResult<TB>> func)
        {
            return self.Match(
                Some: func,
                None: () => new OptionalResult<TB>(Option<TB>.None),
                Fail: ex => new OptionalResult<TB>(ex)
            );
        }

        public static OptionalResult<T> Flatten<T>(this OptionalResult<Option<T>> nestedOptionalResult)
        {
            return nestedOptionalResult.Match(
                Some: opt => opt.Match(
                    Some: val => new OptionalResult<T>(Option<T>.Some(val)),
                    None: () => new OptionalResult<T>(Option<T>.None)
                ),
                None: () => new OptionalResult<T>(Option<T>.None),
                Fail: ex => new OptionalResult<T>(ex)
            );
        }

        // public static Option<T> ToOption<T>(this T? self) where T : struct =>
        //     self == null ? Option<T>.None : Some(self.Value);    }
    }

    public readonly struct Successful
    {
        public override string ToString() => "Prelude.Successful";

        // public Func<T, Successful> ly<T>() => o => Prelude.Successful;
        public static Successful ly<T>(T t) => new();
        public static implicit operator Func<Successful>(Successful s) => () => s;
        public static implicit operator Func<object, Successful>(Successful s) => _ => s;
    }
    
}