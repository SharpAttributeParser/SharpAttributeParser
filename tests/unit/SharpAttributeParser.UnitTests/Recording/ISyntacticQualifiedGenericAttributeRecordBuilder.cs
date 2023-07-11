namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticQualifiedGenericAttributeRecordBuilder : IRecordBuilder<ISyntacticQualifiedGenericAttributeRecord>
{
    public abstract void WithT1Syntax(ExpressionSyntax syntax);
    public abstract void WithT2Syntax(ExpressionSyntax syntax);
}
