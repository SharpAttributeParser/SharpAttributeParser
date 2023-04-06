namespace SharpAttributeParser.Tests;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public class ExampleCases
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Fact]
    public async Task CorrectlyParsed()
    {
        var source = """
            [Example<System.Type>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
            public class Foo { }
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
        public ITypeSymbol T { get; set; } = null!;
        public IReadOnlyList<int> Sequence { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int? Answer { get; set; }

        protected override IEnumerable<(string, DSemanticGenericRecorder)> AddGenericRecorders()
        {
            yield return ("T", Adapters.For(RecordT));
        }

        protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
        {
            yield return ("Sequence", Adapters.For<int>(RecordSequence));
        }

        protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
        {
            yield return ("Name", Adapters.For<string>(RecordName));
            yield return ("Answer", Adapters.For<int>(RecordAnswer));
        }

        private void RecordT(ITypeSymbol t) => T = t;
        private void RecordSequence(IReadOnlyList<int> sequence) => Sequence = sequence;
        private void RecordName(string name) => Name = name;
        private void RecordAnswer(int answer) => Answer = answer;
    }
}
