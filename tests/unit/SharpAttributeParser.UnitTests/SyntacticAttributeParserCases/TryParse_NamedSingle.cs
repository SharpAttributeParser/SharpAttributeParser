namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_NamedSingle
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<Location>()) == false);

        var source = """
            [Named(SingleValue = null)]
            public sealed class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_True_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = (object)4)]
            public sealed class Foo { }
            """;

        await TrueAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(SingleValue = null)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var location = ExpectedLocation.SingleArgument(syntax, 0);

            return new() { Value = null, ValueLocation = location };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(SingleValue = default)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var location = ExpectedLocation.SingleArgument(syntax, 0);

            return new() { Value = null, ValueLocation = location };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(SingleValue = default(object))]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var location = ExpectedLocation.SingleArgument(syntax, 0);

            return new() { Value = null, ValueLocation = location };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Value_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(SingleValue = "42")]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var location = ExpectedLocation.SingleArgument(syntax, 0);

            return new() { Value = "42", ValueLocation = location };
        }
    }

    [AssertionMethod]
    private static async Task TrueAndIdenticalToExpected(ISyntacticAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        SyntacticNamedAttributeRecorder recorder = new();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expected.Value, recorder.SingleValue);
        Assert.True(recorder.SingleValueRecorded);
        Assert.Equal(expected.ValueLocation, recorder.SingleValueLocation);
    }

    [AssertionMethod]
    private static async Task ExpectedOutcomeAndNotRecorded(ISyntacticAttributeParser parser, string source, bool expected)
    {
        SyntacticNamedAttributeRecorder recorder = new();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.Equal(expected, result);

        Assert.False(recorder.SingleValueRecorded);
    }

    [AssertionMethod]
    private static async Task TrueAndNotRecorded(ISyntacticAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, true);

    [AssertionMethod]
    private static async Task FalseAndNotRecorded(ISyntacticAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, false);

    private sealed class ExpectedResult
    {
        public object? Value { get; init; }
        public Location? ValueLocation { get; init; }
    }
}
