namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

public interface ISemanticNamedAttributeRecord
{
    public abstract object? SimpleValue { get; }
    public abstract bool SimpleValueRecorded { get; }

    public abstract IReadOnlyList<object?>? ArrayValue { get; }
    public abstract bool ArrayValueRecorded { get; }
}
