namespace SharpAttributeParser.ExampleCases;

using Microsoft.CodeAnalysis;

using System.Threading.Tasks;

using Xunit;

public sealed class ReadmeExamples
{
    private ISemanticAttributeParser Parser { get; }
    private ISemanticExampleAttributeRecorderFactory RecorderFactory { get; }

    public ReadmeExamples(ISemanticAttributeParser parser, ISemanticExampleAttributeRecorderFactory recorderFactory)
    {
        Parser = parser;
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISemanticAttributeParser parser, ISemanticAttributeRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Fact]
    public async Task TrueAndRecorded()
    {
        var source = """
            [Example<System.Type>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
            public class Foo { }
            """;

        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var typeType = compilation.GetTypeByMetadataName("System.Type");

        var outcome = Target(Parser, recorder, attributeData);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(typeType, result.T);
        Assert.Equal(new[] { 0, 1, 1, 2 }, result.Sequence);
        Assert.Equal("Fib", result.Name);
        Assert.Equal(42, result.Answer);
    }
}
