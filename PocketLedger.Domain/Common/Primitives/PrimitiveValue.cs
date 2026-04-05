namespace PocketLedger.Domain.Common.Primitives;

public static class PrimitiveValue
{
    public static string ToString<T>(T value) =>
        value?.ToString() ?? string.Empty;

    public static bool ValueEquals<T>(T left, T right) =>
        EqualityComparer<T>.Default.Equals(left, right);

    public static int GetValueHashCode<T>(T value) =>
        value is null ? 0 : EqualityComparer<T>.Default.GetHashCode(value);
}