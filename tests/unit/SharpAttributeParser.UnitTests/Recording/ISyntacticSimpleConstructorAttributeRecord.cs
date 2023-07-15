namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticSimpleConstructorAttributeRecord
{
    public abstract ExpressionSyntax? ValueSyntax { get; }
    public abstract bool ValueSyntaxRecorded { get; }
}
