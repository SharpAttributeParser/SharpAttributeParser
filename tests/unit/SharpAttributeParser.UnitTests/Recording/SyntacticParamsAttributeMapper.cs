namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticParamsAttributeMapper : ASyntacticAttributeMapper<ISyntacticParamsAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
    {
        yield return (nameof(ParamsAttribute.Value), Adapters.ForParams(RecordValueSyntax));
    }

    private void RecordValueSyntax(ISyntacticParamsAttributeRecordBuilder builder, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => builder.WithValueSyntax(syntax);
}
