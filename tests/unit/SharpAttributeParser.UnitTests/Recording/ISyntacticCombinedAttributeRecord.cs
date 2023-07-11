namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

public interface ISyntacticCombinedAttributeRecord
{
    public abstract ExpressionSyntax? T1Syntax { get; }
    public abstract bool T1SyntaxRecorded { get; }

    public abstract ExpressionSyntax? T2Syntax { get; }
    public abstract bool T2SyntaxRecorded { get; }

    public abstract ExpressionSyntax? SimpleValueSyntax { get; }
    public abstract bool SimpleValueSyntaxRecorded { get; }

    public abstract ExpressionSyntax? ArrayValueSyntax { get; }
    public abstract bool ArrayValueSyntaxRecorded { get; }

    public abstract OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ParamsValueSyntax { get; }
    public abstract bool ParamsValueSyntaxRecorded { get; }

    public abstract ExpressionSyntax? SimpleNamedValueSyntax { get; }
    public abstract bool SimpleNamedValueSyntaxRecorded { get; }

    public abstract ExpressionSyntax? ArrayNamedValueSyntax { get; }
    public abstract bool ArrayNamedValueSyntaxRecorded { get; }
}
