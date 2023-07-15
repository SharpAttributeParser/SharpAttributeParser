namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticSimpleConstructorAttributeMapper : ASemanticAttributeMapper<ISemanticSimpleConstructorAttributeRecordBuilder>
{
    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(SimpleConstructorAttribute.Value), Adapters.SimpleArgument.ForNullable<object>(RecordValue));
    }

    private void RecordValue(ISemanticSimpleConstructorAttributeRecordBuilder builder, object? value) => builder.WithValue(value);
}
