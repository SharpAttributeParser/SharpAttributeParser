namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticNamedAttributeMapper : ASyntacticAttributeMapper<ISyntacticNamedAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
    {
        yield return (nameof(NamedAttribute.SimpleValue), Adapters.ForNonParams(RecordSimpleValueSyntax));
        yield return (nameof(NamedAttribute.ArrayValue), Adapters.ForNonParams(RecordArrayValueSyntax));
    }

    private void RecordSimpleValueSyntax(ISyntacticNamedAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithSimpleValueSyntax(syntax);
    private void RecordArrayValueSyntax(ISyntacticNamedAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithArrayValueSyntax(syntax);
}
