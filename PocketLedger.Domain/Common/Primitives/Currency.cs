namespace PocketLedger.Domain.Common.Primitives;

public readonly record struct Currency
{
    public string Code { get; }

    private Currency(string code) => Code = code;

    public static readonly Currency Cop = new("COP");
    public static readonly Currency Usd = new("USD");

    public static Currency FromCode(string code) =>
        code.ToUpperInvariant() switch
        {
            "COP" => Cop,
            "USD" => Usd,
            _ => throw new ArgumentOutOfRangeException(nameof(code), $"Unsupported currency: {code}")
        };

    public override string ToString() => Code;
}