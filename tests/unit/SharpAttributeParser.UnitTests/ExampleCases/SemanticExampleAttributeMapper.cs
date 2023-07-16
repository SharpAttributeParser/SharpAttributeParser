namespace SharpAttributeParser.ExampleCases;

using Microsoft.CodeAnalysis;

using OneOf;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticExampleAttributeMapper : ASemanticAttributeMapper<ISemanticExampleAttributeRecordBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.TypeArgument.For(RecordT));
    }

    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(ExampleAttribute<object>.Sequence), Adapters.ArrayArgument.For<int>(RecordSequence));
        yield return (nameof(ExampleAttribute<object>.Name), Adapters.SimpleArgument.For<string>(RecordName));
        yield return (nameof(ExampleAttribute<object>.Answer), Adapters.SimpleArgument.For<int>(RecordAnswer));
    }

    private void RecordT(ISemanticExampleAttributeRecordBuilder builder, ITypeSymbol t) => builder.WithT(t);
    private void RecordSequence(ISemanticExampleAttributeRecordBuilder builder, IReadOnlyList<int> sequence) => builder.WithSequence(sequence);
    private void RecordName(ISemanticExampleAttributeRecordBuilder builder, string name) => builder.WithName(name);
    private void RecordAnswer(ISemanticExampleAttributeRecordBuilder builder, int answer) => builder.WithAnswer(answer);
}
