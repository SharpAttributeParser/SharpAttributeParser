namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticNamedAttributeMapper : ASemanticAttributeMapper<INamedAttributeDataBuilder>
{
    protected override IEnumerable<(string, DSemanticAttributeArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(NamedAttribute.SimpleValue), Adapters.Simple.ForNullable<object>(RecordSimpleValue));
        yield return (nameof(NamedAttribute.ArrayValue), Adapters.Collection.ForNullable<object>(RecordArrayValue));
    }

    private void RecordSimpleValue(INamedAttributeDataBuilder builder, object? value) => builder.WithSimpleValue(value, Location.None);
    private void RecordArrayValue(INamedAttributeDataBuilder builder, IReadOnlyList<object?>? value) => builder.WithArrayValue(value, CollectionLocation.None);
}
