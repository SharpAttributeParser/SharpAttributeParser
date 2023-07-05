namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using OneOf;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticExampleAttributeMapper : ASemanticAttributeMapper<IExampleAttributeDataBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.Type.For(RecordT));
    }

    protected override IEnumerable<(string, DSemanticAttributeArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(ExampleAttribute<object>.Sequence), Adapters.Collection.For<int>(RecordSequence));
        yield return (nameof(ExampleAttribute<object>.Name), Adapters.Simple.For<string>(RecordName));
        yield return (nameof(ExampleAttribute<object>.Answer), Adapters.Simple.For<int>(RecordAnswer));
    }

    private void RecordT(IExampleAttributeDataBuilder builder, ITypeSymbol t) => builder.WithT(t);
    private void RecordSequence(IExampleAttributeDataBuilder builder, IReadOnlyList<int> sequence) => builder.WithSequence(sequence);
    private void RecordName(IExampleAttributeDataBuilder builder, string name) => builder.WithName(name);
    private void RecordAnswer(IExampleAttributeDataBuilder builder, int answer) => builder.WithAnswer(answer);
}
