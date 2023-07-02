namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_NamedSingle
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISemanticAttributeParser parser)
    {
        var recorder = Mock.Of<ISemanticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<object?>()) == false);

        var source = """
            [Named(SingleValue = null)]
            public sealed class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.False(result);
    }

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
            [Named(4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_True_NotRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = (object)4)]
            public sealed class Foo { }
            """;

        await TrueAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Named(SingleValue = null)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static object? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Named(SingleValue = default)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static object? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Named(SingleValue = default(object[]))]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static object? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Value_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Named(SingleValue = "42")]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static object? expected(Compilation compilation) => "42";
    }

    [AssertionMethod]
    private static async Task TrueAndIdenticalToExpected(ISemanticAttributeParser parser, string source, Func<Compilation, object?> expectedDelegate)
    {
        SemanticNamedAttributeRecorder recorder = new();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expected, recorder.SingleValue);
        Assert.True(recorder.SingleValueRecorded);
    }

    [AssertionMethod]
    private static async Task ExpectedOutcomeAndNotRecorded(ISemanticAttributeParser parser, string source, bool expected)
    {
        SemanticNamedAttributeRecorder recorder = new();

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData);

        Assert.Equal(expected, result);

        Assert.False(recorder.SingleValueRecorded);
    }

    [AssertionMethod]
    private static async Task TrueAndNotRecorded(ISemanticAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, true);

    [AssertionMethod]
    private static async Task FalseAndNotRecorded(ISemanticAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, false);
}
