namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class GenericAttributeMapper : AAttributeMapper<IGenericAttributeRecordBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.TypeArgument.For(RecordT1));
        yield return (1, Adapters.TypeArgument.For(RecordT2));
    }

    private void RecordT1(IGenericAttributeRecordBuilder builder, ITypeSymbol value, ExpressionSyntax syntax)
    {
        builder.WithT1(value);
        builder.WithT1Syntax(syntax);
    }

    private void RecordT2(IGenericAttributeRecordBuilder builder, ITypeSymbol value, ExpressionSyntax syntax)
    {
        builder.WithT2(value);
        builder.WithT2Syntax(syntax);
    }
}
