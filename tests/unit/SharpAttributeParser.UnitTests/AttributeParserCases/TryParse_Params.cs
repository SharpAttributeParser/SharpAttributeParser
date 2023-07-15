namespace SharpAttributeParser.AttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Recording;

using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params
{
    private IParamsAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Params(IParamsAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(IAttributeParser parser, IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task FalseReturningRecorder_FalseAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(4)]
            public class Foo { }
            """;

        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(new object?[] { 4 }, result.Value);
        Assert.True(result.ValueRecorded);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(nonExisting: 4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorParamsArgument_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [Params((string)4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArrayArgument_FalseAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [Params((object[])4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [AssertionMethod]
    private async Task FalseAndNotRecorded(IAttributeParser parser, string source)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.False(outcome);

        Assert.False(result.ValueRecorded);
    }
}
