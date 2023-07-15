namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class NamedAttributeMapper : AAttributeMapper<INamedAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(NamedAttribute.SimpleValue), Adapters.SimpleArgument.ForNullable<object>(RecordSimpleValue));
        yield return (nameof(NamedAttribute.ArrayValue), Adapters.ArrayArgument.NonParams.ForNullable<object>(RecordArrayValue));
    }

    private void RecordSimpleValue(INamedAttributeRecordBuilder builder, object? value, ExpressionSyntax syntax)
    {
        builder.WithSimpleValue(value);
        builder.WithSimpleValueSyntax(syntax);
    }

    private void RecordArrayValue(INamedAttributeRecordBuilder builder, IReadOnlyList<object?>? value, ExpressionSyntax syntax)
    {
        builder.WithArrayValue(value);
        builder.WithArrayValueSyntax(syntax);
    }
}
