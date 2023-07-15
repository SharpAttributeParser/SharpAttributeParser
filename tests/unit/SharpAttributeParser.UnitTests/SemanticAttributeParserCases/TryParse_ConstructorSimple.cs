namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_SimpleConstructorSimple
{
    private ISemanticSimpleConstructorAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_SimpleConstructorSimple(ISemanticSimpleConstructorAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISemanticAttributeParser parser, ISemanticAttributeRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        var recorder = Mock.Of<ISemanticAttributeRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>()) == false);

        var source = """
            [SimpleConstructor(null)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);
    }

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
            [SimpleConstructor(3, 4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_FalseAndNotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [SimpleConstructor((string)4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [SimpleConstructor(null)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static object? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [SimpleConstructor(default)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static object? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [SimpleConstructor(default(object))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static object? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Value_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [SimpleConstructor("42")]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static object? expected(Compilation compilation) => "42";
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(ISemanticAttributeParser parser, string source, Func<Compilation, object?> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var outcome = Target(parser, recorder, attributeData);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected, result.Value);
        Assert.True(result.ValueRecorded);
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
