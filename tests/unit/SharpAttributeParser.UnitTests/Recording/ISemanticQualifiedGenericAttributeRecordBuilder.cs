namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

public interface ISemanticQualifiedGenericAttributeRecordBuilder : IRecordBuilder<ISemanticQualifiedGenericAttributeRecord>
{
    public abstract void WithT1(ITypeSymbol t1);
    public abstract void WithT2(ITypeSymbol t2);
}
