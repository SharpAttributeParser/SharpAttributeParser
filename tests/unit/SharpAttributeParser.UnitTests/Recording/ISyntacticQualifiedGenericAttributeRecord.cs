namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticQualifiedGenericAttributeRecord
{
    public abstract ExpressionSyntax? T1Syntax { get; }
    public abstract bool T1SyntaxRecorded { get; }

    public abstract ExpressionSyntax? T2Syntax { get; }
    public abstract bool T2SyntaxRecorded { get; }
}
