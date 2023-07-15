namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class ArrayConstructorAttributeMapper : AAttributeMapper<IArrayConstructorAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(ArrayConstructorAttribute.Value), Adapters.ArrayArgument.NonParams.ForNullable<object>(RecordValue));
    }

    private void RecordValue(IArrayConstructorAttributeRecordBuilder builder, IReadOnlyList<object?>? value, ExpressionSyntax syntax)
    {
        builder.WithValue(value);
        builder.WithValueSyntax(syntax);
    }
}
