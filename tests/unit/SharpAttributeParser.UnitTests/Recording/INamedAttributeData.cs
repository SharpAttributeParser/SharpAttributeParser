namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public interface INamedAttributeData
{
    public abstract object? SimpleValue { get; }
    public abstract bool SimpleValueRecorded { get; }
    public abstract Location SimpleValueLocation { get; }

    public abstract IReadOnlyList<object?>? ArrayValue { get; }
    public abstract bool ArrayValueRecorded { get; }
    public abstract CollectionLocation ArrayValueLocation { get; }
}
