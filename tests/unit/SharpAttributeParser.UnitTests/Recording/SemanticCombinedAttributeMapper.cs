namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticCombinedAttributeMapper : ASemanticAttributeMapper<ICombinedAttributeDataBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.Type.For(RecordT1));
        yield return (1, Adapters.Type.For(RecordT2));
    }

    protected override IEnumerable<(string, DSemanticAttributeArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(CombinedAttribute<object, object>.SimpleValue), Adapters.Simple.ForNullable<object>(RecordSimpleValue));
        yield return (nameof(CombinedAttribute<object, object>.ArrayValue), Adapters.Collection.ForNullable<object>(RecordArrayValue));
        yield return (nameof(CombinedAttribute<object, object>.ParamsValue), Adapters.Collection.ForNullable<object>(RecordParamsValue));

        yield return (nameof(CombinedAttribute<object, object>.SimpleNamedValue), Adapters.Simple.ForNullable<object>(RecordNamedSimpleValue));
        yield return (nameof(CombinedAttribute<object, object>.ArrayNamedValue), Adapters.Collection.ForNullable<object>(RecordNamedArrayValue));
    }

    private void RecordT1(ICombinedAttributeDataBuilder builder, ITypeSymbol t1) => builder.WithT1(t1, Location.None);
    private void RecordT2(ICombinedAttributeDataBuilder builder, ITypeSymbol t2) => builder.WithT2(t2, Location.None);

    private void RecordSimpleValue(ICombinedAttributeDataBuilder builder, object? value) => builder.WithSimpleValue(value, Location.None);
    private void RecordArrayValue(ICombinedAttributeDataBuilder builder, IReadOnlyList<object?>? value) => builder.WithArrayValue(value, CollectionLocation.None);
    private void RecordParamsValue(ICombinedAttributeDataBuilder builder, IReadOnlyList<object?>? value) => builder.WithParamsValue(value, CollectionLocation.None);

    private void RecordNamedSimpleValue(ICombinedAttributeDataBuilder builder, object? value) => builder.WithSimpleNamedValue(value, Location.None);
    private void RecordNamedArrayValue(ICombinedAttributeDataBuilder builder, IReadOnlyList<object?>? value) => builder.WithArrayNamedValue(value, CollectionLocation.None);
}
