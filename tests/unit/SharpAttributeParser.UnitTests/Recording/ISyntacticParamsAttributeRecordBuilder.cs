namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System.Collections.Generic;

internal interface ISyntacticParamsAttributeRecordBuilder : IRecordBuilder<ISyntacticParamsAttributeRecord>
{
    public abstract void WithValueSyntax(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax);
}
