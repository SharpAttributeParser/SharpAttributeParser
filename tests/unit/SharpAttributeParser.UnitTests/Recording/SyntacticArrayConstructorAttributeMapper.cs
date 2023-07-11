namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticArrayConstructorAttributeMapper : ASyntacticAttributeMapper<ISyntacticArrayConstructorAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
    {
        yield return (nameof(ArrayConstructorAttribute.Value), Adapters.ForNonParams(RecordValueSyntax));
    }

    private void RecordValueSyntax(ISyntacticArrayConstructorAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithValueSyntax(syntax);
}
