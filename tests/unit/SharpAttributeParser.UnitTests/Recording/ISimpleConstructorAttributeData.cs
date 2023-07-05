namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

public interface ISimpleConstructorAttributeData
{
    public abstract object? Value { get; }
    public abstract bool ValueRecorded { get; }
    public abstract Location ValueLocation { get; }
}
