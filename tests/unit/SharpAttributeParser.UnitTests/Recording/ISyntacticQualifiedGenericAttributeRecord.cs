namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticQualifiedGenericAttributeRecord : ISemanticQualifiedGenericAttributeRecord
{
    public abstract ExpressionSyntax T1Syntax { get; }
    public abstract ExpressionSyntax T2Syntax { get; }
}
