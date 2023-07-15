namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticArrayConstructorAttributeRecord
{
    public abstract ExpressionSyntax? ValueSyntax { get; }
    public abstract bool ValueSyntaxRecorded { get; }
}
