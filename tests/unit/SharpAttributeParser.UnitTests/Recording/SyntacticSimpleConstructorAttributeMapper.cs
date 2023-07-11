namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticSimpleConstructorAttributeMapper : ASyntacticAttributeMapper<ISyntacticSimpleConstructorAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
    {
        yield return (nameof(SimpleConstructorAttribute.Value), Adapters.ForNonParams(RecordValueSyntax));
    }

    private void RecordValueSyntax(ISyntacticSimpleConstructorAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithValueSyntax(syntax);
}
