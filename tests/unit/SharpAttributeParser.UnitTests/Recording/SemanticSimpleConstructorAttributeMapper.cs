namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticSimpleConstructorAttributeMapper : ASemanticAttributeMapper<ISimpleConstructorAttributeDataBuilder>
{
    protected override IEnumerable<(string, DSemanticAttributeArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(SimpleConstructorAttribute.Value), Adapters.Simple.ForNullable<object>(RecordValue));
    }

    private void RecordValue(ISimpleConstructorAttributeDataBuilder builder, object? value) => builder.WithValue(value, Location.None);
}
