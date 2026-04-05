namespace PocketLedger.Domain.Common.Primitives;

public interface IPrimitiveValue
{
    object RawValue { get; }
}

public interface IPrimitiveValue<out T> : IPrimitiveValue
{
    T Value { get; }
}

public interface IStringValue : IPrimitiveValue<string> { }

public interface IIntValue : IPrimitiveValue<int> { }
public interface IDecimalValue : IPrimitiveValue<decimal> { }

public interface ILongValue : IPrimitiveValue<long> { }

public interface IGuidValue : IPrimitiveValue<Guid> { }

public interface IBytesValue : IPrimitiveValue<byte[]> { }