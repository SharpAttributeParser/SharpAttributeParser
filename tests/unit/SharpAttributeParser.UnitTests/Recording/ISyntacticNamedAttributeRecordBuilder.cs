namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

internal interface ISyntacticNamedAttributeRecordBuilder : IRecordBuilder<ISyntacticNamedAttributeRecord>
{
    public abstract void WithSimpleValue(object? value, ExpressionSyntax syntax);
    public abstract void WithArrayValue(IReadOnlyList<object?>? value, ExpressionSyntax syntax);
}
