namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

public interface ISemanticQualifiedGenericAttributeRecord
{
    public abstract ITypeSymbol? T1 { get; }
    public abstract bool T1Recorded { get; }

    public abstract ITypeSymbol? T2 { get; }
    public abstract bool T2Recorded { get; }
}
