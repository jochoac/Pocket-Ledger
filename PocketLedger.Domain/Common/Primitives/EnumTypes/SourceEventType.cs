using LanguageExt;
using PocketLedger.Domain.Common.ErrorTypes;

namespace PocketLedger.Domain.Common.Primitives.EnumTypes;

using Primitives;

public enum SourceEventTypeValue
{
    Wallet = 1,
    Sms = 2,
    Email = 3,
    Webhook = 4,
    Manual = 5,
    Import = 6
}

public readonly record struct SourceEventType(SourceEventTypeValue Value) : IPrimitiveValue<SourceEventTypeValue>
{
    public object RawValue => Value;

    public static SourceEventType Wallet => new(SourceEventTypeValue.Wallet);
    public static SourceEventType Sms => new(SourceEventTypeValue.Sms);
    public static SourceEventType Email => new(SourceEventTypeValue.Email);
    public static SourceEventType Webhook => new(SourceEventTypeValue.Webhook);
    public static SourceEventType Manual => new(SourceEventTypeValue.Manual);
    public static SourceEventType Import => new(SourceEventTypeValue.Import);

    public static Validation<Error, SourceEventType> Parse(string value) =>
        value.Trim().ToLowerInvariant() switch
        {
            "manual" => Manual,
            "wallet" => Wallet,
            "sms" => Sms,
            _ => Errors.InvalidValue<SourceEventType>(value, $"Invalid source event type '{value}'.")
        };

    public override string ToString() => Value.ToString();

    public static implicit operator SourceEventTypeValue(SourceEventType value) => value.Value;
    public static implicit operator SourceEventType(SourceEventTypeValue value) => new(value);
}