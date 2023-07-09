namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticGenericAttributeRecord : ISemanticGenericAttributeRecord
{
    public abstract ExpressionSyntax T1Syntax { get; }
    public abstract ExpressionSyntax T2Syntax { get; }
}
