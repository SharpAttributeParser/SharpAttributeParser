namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticNamedAttributeMapper : ASemanticAttributeMapper<ISemanticNamedAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(NamedAttribute.SimpleValue), Adapters.SimpleArgument.ForNullable<object>(RecordSimpleValue));
        yield return (nameof(NamedAttribute.ArrayValue), Adapters.ArrayArgument.ForNullable<object>(RecordArrayValue));
    }

    private void RecordSimpleValue(ISemanticNamedAttributeRecordBuilder builder, object? value) => builder.WithSimpleValue(value);
    private void RecordArrayValue(ISemanticNamedAttributeRecordBuilder builder, IReadOnlyList<object?>? value) => builder.WithArrayValue(value);
}
