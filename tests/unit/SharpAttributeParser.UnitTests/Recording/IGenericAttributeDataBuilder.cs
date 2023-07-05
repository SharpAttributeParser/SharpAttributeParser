namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

public interface IGenericAttributeDataBuilder : IAttributeDataBuilder<IGenericAttributeData>
{
    public abstract void WithT1(ITypeSymbol t1, Location t1Location);
    public abstract void WithT2(ITypeSymbol t2, Location t2Location);
}
