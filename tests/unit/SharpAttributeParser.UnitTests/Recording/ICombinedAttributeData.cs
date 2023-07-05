namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public interface ICombinedAttributeData
{
    public abstract ITypeSymbol? T1 { get; }
    public abstract bool T1Recorded { get; }
    public abstract Location T1Location { get; }

    public abstract ITypeSymbol? T2 { get; }
    public abstract bool T2Recorded { get; }
    public abstract Location T2Location { get; }

    public abstract object? SimpleValue { get; }
    public abstract bool SimpleValueRecorded { get; }
    public abstract Location SimpleValueLocation { get; }

    public abstract IReadOnlyList<object?>? ArrayValue { get; }
    public abstract bool ArrayValueRecorded { get; }
    public abstract CollectionLocation ArrayValueLocation { get; }

    public abstract IReadOnlyList<object?>? ParamsValue { get; }
    public abstract bool ParamsValueRecorded { get; }
    public abstract CollectionLocation ParamsValueLocation { get; }

    public abstract object? SimpleNamedValue { get; }
    public abstract bool SimpleNamedValueRecorded { get; }
    public abstract Location SimpleNamedValueLocation { get; }

    public abstract IReadOnlyList<object?>? ArrayNamedValue { get; }
    public abstract bool ArrayNamedValueRecorded { get; }
    public abstract CollectionLocation ArrayNamedValueLocation { get; }
}
