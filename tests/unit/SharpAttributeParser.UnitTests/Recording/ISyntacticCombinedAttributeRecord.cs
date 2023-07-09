namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

public interface ISyntacticCombinedAttributeRecord : ISemanticCombinedAttributeRecord
{
    public abstract ExpressionSyntax T1Syntax { get; }
    public abstract ExpressionSyntax T2Syntax { get; }

    public abstract ExpressionSyntax SimpleValueSyntax { get; }
    public abstract ExpressionSyntax ArrayValueSyntax { get; }
    public abstract IReadOnlyList<ExpressionSyntax> ParamsValueSyntax { get; }

    public abstract ExpressionSyntax SimpleNamedValueSyntax { get; }
    public abstract ExpressionSyntax ArrayNamedValueSyntax { get; }
}
