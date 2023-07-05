namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticQualifiedGenericAttributeMapper : ASemanticAttributeMapper<IQualifiedGenericAttributeDataBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.Type.For(RecordT1));
        yield return (1, Adapters.Type.For(RecordT2));
    }

    private void RecordT1(IQualifiedGenericAttributeDataBuilder builder, ITypeSymbol value) => builder.WithT1(value, Location.None);
    private void RecordT2(IQualifiedGenericAttributeDataBuilder builder, ITypeSymbol value) => builder.WithT2(value, Location.None);
}
