namespace PocketLedger.Domain.Common.LanguageExt.Extensions;

public static class EnumerableExtensions
{
    public static string Combine(this IEnumerable<string> src, string delimiter = "\n") =>
        !src.Any() 
            ? string.Empty 
            : src.Aggregate((i, j) => i + delimiter + j);
}