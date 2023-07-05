namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticArrayConstructorAttributeMapper : ASemanticAttributeMapper<IArrayConstructorAttributeDataBuilder>
{
    protected override IEnumerable<(string, DSemanticAttributeArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(ArrayConstructorAttribute.Value), Adapters.Collection.ForNullable<object>(RecordValue));
    }

    private void RecordValue(IArrayConstructorAttributeDataBuilder builder, IReadOnlyList<object?>? value) => builder.WithValue(value, CollectionLocation.None);
}
