namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

internal interface ISyntacticCombinedAttributeRecordBuilder : IRecordBuilder<ISyntacticCombinedAttributeRecord>
{
    public abstract void WithT1(ITypeSymbol t1, ExpressionSyntax syntax);
    public abstract void WithT2(ITypeSymbol t2, ExpressionSyntax syntax);

    public abstract void WithSimpleValue(object? value, ExpressionSyntax syntax);
    public abstract void WithArrayValue(IReadOnlyList<object?>? value, ExpressionSyntax syntax);
    public abstract void WithParamsValue(IReadOnlyList<object?>? value, IReadOnlyList<ExpressionSyntax> syntax);

    public abstract void WithSimpleNamedValue(object? value, ExpressionSyntax syntax);
    public abstract void WithArrayNamedValue(IReadOnlyList<object?>? value, ExpressionSyntax syntax);
}
