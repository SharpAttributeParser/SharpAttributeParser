namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class CombinedAttributeMapper : AAttributeMapper<ICombinedAttributeRecordBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.TypeArgument.For(RecordT1));
        yield return (1, Adapters.TypeArgument.For(RecordT2));
    }

    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(CombinedAttribute<object, object>.SimpleValue), Adapters.SimpleArgument.ForNullable<object>(RecordSimpleValue));
        yield return (nameof(CombinedAttribute<object, object>.ArrayValue), Adapters.ArrayArgument.NonParams.ForNullable<object>(RecordArrayValue));
        yield return (nameof(CombinedAttribute<object, object>.ParamsValue), Adapters.ArrayArgument.Params.ForNullableElements<object>(RecordParamsValue));

        yield return (nameof(CombinedAttribute<object, object>.SimpleNamedValue), Adapters.SimpleArgument.ForNullable<object>(RecordSimpleNamedValue));
        yield return (nameof(CombinedAttribute<object, object>.ArrayNamedValue), Adapters.ArrayArgument.NonParams.ForNullable<object>(RecordArrayNamedValue));
    }

    private void RecordT1(ICombinedAttributeRecordBuilder builder, ITypeSymbol t1, ExpressionSyntax syntax)
    {
        builder.WithT1(t1);
        builder.WithT1Syntax(syntax);
    }

    private void RecordT2(ICombinedAttributeRecordBuilder builder, ITypeSymbol t2, ExpressionSyntax syntax)
    {
        builder.WithT2(t2);
        builder.WithT2Syntax(syntax);
    }

    private void RecordSimpleValue(ICombinedAttributeRecordBuilder builder, object? value, ExpressionSyntax syntax)
    {
        builder.WithSimpleValue(value);
        builder.WithSimpleValueSyntax(syntax);
    }

    private void RecordArrayValue(ICombinedAttributeRecordBuilder builder, IReadOnlyList<object?>? value, ExpressionSyntax syntax)
    {
        builder.WithArrayValue(value);
        builder.WithArrayValueSyntax(syntax);
    }

    private void RecordParamsValue(ICombinedAttributeRecordBuilder builder, IReadOnlyList<object?>? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        builder.WithParamsValue(value);
        builder.WithParamsValueSyntax(syntax);
    }

    private void RecordSimpleNamedValue(ICombinedAttributeRecordBuilder builder, object? value, ExpressionSyntax syntax)
    {
        builder.WithSimpleNamedValue(value);
        builder.WithSimpleNamedValueSyntax(syntax);
    }

    private void RecordArrayNamedValue(ICombinedAttributeRecordBuilder builder, IReadOnlyList<object?>? value, ExpressionSyntax syntax)
    {
        builder.WithArrayNamedValue(value);
        builder.WithArrayNamedValueSyntax(syntax);
    }
}
