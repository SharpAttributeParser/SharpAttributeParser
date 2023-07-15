namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticCombinedAttributeMapper : ASemanticAttributeMapper<ISemanticCombinedAttributeRecordBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.TypeArgument.For(RecordT1));
        yield return (1, Adapters.TypeArgument.For(RecordT2));
    }

    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(CombinedAttribute<object, object>.SimpleValue), Adapters.SimpleArgument.ForNullable<object>(RecordSimpleValue));
        yield return (nameof(CombinedAttribute<object, object>.ArrayValue), Adapters.ArrayArgument.ForNullable<object>(RecordArrayValue));
        yield return (nameof(CombinedAttribute<object, object>.ParamsValue), Adapters.ArrayArgument.ForNullable<object>(RecordParamsValue));

        yield return (nameof(CombinedAttribute<object, object>.SimpleNamedValue), Adapters.SimpleArgument.ForNullable<object>(RecordSimpleNamedValue));
        yield return (nameof(CombinedAttribute<object, object>.ArrayNamedValue), Adapters.ArrayArgument.ForNullable<object>(RecordArrayNamedValue));
    }

    private void RecordT1(ISemanticCombinedAttributeRecordBuilder builder, ITypeSymbol t1) => builder.WithT1(t1);
    private void RecordT2(ISemanticCombinedAttributeRecordBuilder builder, ITypeSymbol t2) => builder.WithT2(t2);

    private void RecordSimpleValue(ISemanticCombinedAttributeRecordBuilder builder, object? value) => builder.WithSimpleValue(value);
    private void RecordArrayValue(ISemanticCombinedAttributeRecordBuilder builder, IReadOnlyList<object?>? value) => builder.WithArrayValue(value);
    private void RecordParamsValue(ISemanticCombinedAttributeRecordBuilder builder, IReadOnlyList<object?>? value) => builder.WithParamsValue(value);

    private void RecordSimpleNamedValue(ISemanticCombinedAttributeRecordBuilder builder, object? value) => builder.WithSimpleNamedValue(value);
    private void RecordArrayNamedValue(ISemanticCombinedAttributeRecordBuilder builder, IReadOnlyList<object?>? value) => builder.WithArrayNamedValue(value);
}
