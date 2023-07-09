namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticSimpleConstructorAttributeRecord : ISemanticSimpleConstructorAttributeRecord
{
    public abstract ExpressionSyntax ValueSyntax { get; }
}
