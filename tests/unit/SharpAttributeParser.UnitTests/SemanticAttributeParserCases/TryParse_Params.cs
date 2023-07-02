namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_False_NotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_False_NotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(nonExisting: 4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorParamsArgument_False_NotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((string)4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArrayArgument_False_NotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((object[])4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [AssertionMethod]
    private static async Task FalseAndNotRecorded(ISemanticAttributeParser parser, string source)
    {
        SemanticParamsAttributeRecorder recorder = new();

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);

        Assert.False(recorder.ValueRecorded);
    }
}
