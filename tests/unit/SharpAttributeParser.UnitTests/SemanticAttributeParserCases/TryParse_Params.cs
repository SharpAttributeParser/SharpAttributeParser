namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Recording;

using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params
{
    private ISemanticParamsAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Params(ISemanticParamsAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISemanticAttributeParser parser, ISemanticAttributeRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(nonExisting: 4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorParamsArgument_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((string)4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArrayArgument_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((object[])4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [AssertionMethod]
    private async Task FalseAndNotRecorded(ISemanticAttributeParser parser, string source)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData);
        var result = recorder.GetRecord();

        Assert.False(outcome);

        Assert.False(result.ValueRecorded);
    }
}
