namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticArrayConstructorAttributeMapper : ASemanticAttributeMapper<ISemanticArrayConstructorAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(ArrayConstructorAttribute.Value), Adapters.ArrayArgument.ForNullable<object>(RecordValue));
    }

    private void RecordValue(ISemanticArrayConstructorAttributeRecordBuilder builder, IReadOnlyList<object?>? value) => builder.WithValue(value);
}
