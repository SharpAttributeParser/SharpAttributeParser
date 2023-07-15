namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SimpleConstructorAttributeMapper : AAttributeMapper<ISimpleConstructorAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(SimpleConstructorAttribute.Value), Adapters.SimpleArgument.ForNullable<object>(RecordValue));
    }

    private void RecordValue(ISimpleConstructorAttributeRecordBuilder builder, object? value, ExpressionSyntax syntax)
    {
        builder.WithValue(value);
        builder.WithValueSyntax(syntax);
    }
}
