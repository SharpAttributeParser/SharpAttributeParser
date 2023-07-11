namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

internal interface ISyntacticCombinedAttributeRecordBuilder : IRecordBuilder<ISyntacticCombinedAttributeRecord>
{
    public abstract void WithT1Syntax(ExpressionSyntax syntax);
    public abstract void WithT2Syntax(ExpressionSyntax syntax);

    public abstract void WithSimpleValueSyntax(ExpressionSyntax syntax);
    public abstract void WithArrayValueSyntax(ExpressionSyntax syntax);
    public abstract void WithParamsValueSyntax(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax);

    public abstract void WithSimpleNamedValueSyntax(ExpressionSyntax syntax);
    public abstract void WithArrayNamedValueSyntax(ExpressionSyntax syntax);
}
