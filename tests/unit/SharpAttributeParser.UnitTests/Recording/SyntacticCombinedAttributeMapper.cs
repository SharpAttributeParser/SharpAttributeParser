﻿namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticCombinedAttributeMapper : ASyntacticAttributeMapper<ISyntacticCombinedAttributeRecordBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.ForType(RecordT1Syntax));
        yield return (1, Adapters.ForType(RecordT2Syntax));
    }

    protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
    {
        yield return (nameof(CombinedAttribute<object, object>.SimpleValue), Adapters.ForNonParams(RecordSimpleValueSyntax));
        yield return (nameof(CombinedAttribute<object, object>.ArrayValue), Adapters.ForNonParams(RecordArrayValueSyntax));
        yield return (nameof(CombinedAttribute<object, object>.ParamsValue), Adapters.ForParams(RecordParamsValueSyntax));

        yield return (nameof(CombinedAttribute<object, object>.SimpleNamedValue), Adapters.ForNonParams(RecordSimpleNamedValueSyntax));
        yield return (nameof(CombinedAttribute<object, object>.ArrayNamedValue), Adapters.ForNonParams(RecordArrayNamedValueSyntax));
    }

    private void RecordT1Syntax(ISyntacticCombinedAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithT1Syntax(syntax);
    private void RecordT2Syntax(ISyntacticCombinedAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithT2Syntax(syntax);

    private void RecordSimpleValueSyntax(ISyntacticCombinedAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithSimpleValueSyntax(syntax);
    private void RecordArrayValueSyntax(ISyntacticCombinedAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithArrayValueSyntax(syntax);
    private void RecordParamsValueSyntax(ISyntacticCombinedAttributeRecordBuilder builder, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => builder.WithParamsValueSyntax(syntax);

    private void RecordSimpleNamedValueSyntax(ISyntacticCombinedAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithSimpleNamedValueSyntax(syntax);
    private void RecordArrayNamedValueSyntax(ISyntacticCombinedAttributeRecordBuilder builder, ExpressionSyntax syntax) => builder.WithArrayNamedValueSyntax(syntax);
}