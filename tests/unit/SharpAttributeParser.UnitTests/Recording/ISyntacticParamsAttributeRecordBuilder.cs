namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

internal interface ISyntacticParamsAttributeRecordBuilder : IRecordBuilder<ISyntacticParamsAttributeRecord>
{
    public abstract void WithValue(IReadOnlyList<object?>? value, IReadOnlyList<ExpressionSyntax> syntax);
}
