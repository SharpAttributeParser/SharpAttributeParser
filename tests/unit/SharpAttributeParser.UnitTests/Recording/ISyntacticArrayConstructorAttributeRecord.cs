namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticArrayConstructorAttributeRecord : ISemanticArrayConstructorAttributeRecord
{
    public abstract ExpressionSyntax ValueSyntax { get; }
}
