namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticGenericAttributeMapper : ASyntacticAttributeMapper<ISyntacticGenericAttributeRecordBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.TypeArgument.For(RecordT1Syntax));
        yield return (1, Adapters.TypeArgument.For(RecordT2Syntax));
    }

    private void RecordT1Syntax(ISyntacticGenericAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithT1Syntax(syntax);
    private void RecordT2Syntax(ISyntacticGenericAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithT2Syntax(syntax);
}
