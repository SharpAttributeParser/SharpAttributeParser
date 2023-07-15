namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

public interface ISyntacticNamedAttributeRecord
{
    public abstract ExpressionSyntax? SimpleValueSyntax { get; }
    public abstract bool SimpleValueSyntaxRecorded { get; }

    public abstract ExpressionSyntax? ArrayValueSyntax { get; }
    public abstract bool ArrayValueSyntaxRecorded { get; }
}
