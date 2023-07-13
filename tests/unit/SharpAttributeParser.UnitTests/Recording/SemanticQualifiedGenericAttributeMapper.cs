namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticQualifiedGenericAttributeMapper : ASemanticAttributeMapper<ISemanticQualifiedGenericAttributeRecordBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.TypeArgument.For(RecordT1));
        yield return (1, Adapters.TypeArgument.For(RecordT2));
    }

    private void RecordT1(ISemanticQualifiedGenericAttributeRecordBuilder builder, ITypeSymbol value) => builder.WithT1(value);
    private void RecordT2(ISemanticQualifiedGenericAttributeRecordBuilder builder, ITypeSymbol value) => builder.WithT2(value);
}
