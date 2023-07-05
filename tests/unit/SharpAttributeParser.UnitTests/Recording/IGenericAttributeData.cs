namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

public interface IGenericAttributeData
{
    public abstract ITypeSymbol? T1 { get; }
    public abstract bool T1Recorded { get; }
    public abstract Location T1Location { get; }

    public abstract ITypeSymbol? T2 { get; }
    public abstract bool T2Recorded { get; }
    public abstract Location T2Location { get; }
}
