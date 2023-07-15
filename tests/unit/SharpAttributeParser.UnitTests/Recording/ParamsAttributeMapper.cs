namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class ParamsAttributeMapper : AAttributeMapper<IParamsAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(ParamsAttribute.Value), Adapters.ArrayArgument.Params.ForNullable<object>(RecordValue));
    }

    private void RecordValue(IParamsAttributeRecordBuilder builder, IReadOnlyList<object?>? value, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
    {
        builder.WithValue(value);
        builder.WithValueSyntax(syntax);
    }
}
