namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class ExampleCases
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Fact]
    public async Task CorrectlyParsed()
    {
        var source = """
            [Example<System.Type>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
            public sealed class Foo { }
            """;

        var parser = DependencyInjection.GetRequiredService<ISemanticAttributeParser>();

        ExampleRecorder recorder = new();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var typeType = compilation.GetTypeByMetadataName("System.Type");

        var actual = Target(parser, recorder, attributeData);

        Assert.True(actual);

        Assert.Equal(typeType, recorder.T);
        Assert.Equal(new[] { 0, 1, 1, 2 }, recorder.Sequence);
        Assert.Equal("Fib", recorder.Name);
        Assert.Equal(42, recorder.Answer);
    }

    private sealed class ExampleRecorder : ASemanticArgumentRecorder
    {
        public ITypeSymbol? T { get; set; }
        public IReadOnlyList<int>? Sequence { get; set; }
        public string? Name { get; set; }
        public int? Answer { get; set; }

        protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders()
        {
            yield return (0, Adapters.For(RecordT));
        }

        protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
        {
            yield return (nameof(ExampleAttribute<object>.Sequence), Adapters.For<int>(RecordSequence));
        }

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return (nameof(ExampleAttribute<object>.Name), Adapters.For<string>(RecordName));
            yield return (nameof(ExampleAttribute<object>.Answer), Adapters.For<int>(RecordAnswer));
        }

        private void RecordT(ITypeSymbol t) => T = t;
        private void RecordSequence(IReadOnlyList<int> sequence) => Sequence = sequence;
        private void RecordName(string name) => Name = name;
        private void RecordAnswer(int answer) => Answer = answer;
    }
}
