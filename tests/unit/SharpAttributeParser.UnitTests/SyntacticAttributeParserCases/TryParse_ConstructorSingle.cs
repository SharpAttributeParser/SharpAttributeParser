namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_ConstructorSingle
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticArgumentRecorder>(static (recorder) => recorder.TryRecordConstructorArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>(), It.IsAny<Location>()) == false);

        var source = """
            [ConstructorSingle(null)]
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
            [ConstructorSingle(3, 4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_False_NotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ConstructorSingle((string)4)]
            public sealed class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ConstructorSingle(null)]
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
            [ConstructorSingle(default)]
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
            [ConstructorSingle(default(object))]
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
            [ConstructorSingle("42")]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var location = ExpectedLocation.SingleArgument(syntax, 0);

            return new() { Value = "42", ValueLocation = location };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Labelled_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ConstructorSingle(value: "42")]
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
        SyntacticConstructorSingleAttributeRecorder recorder = new();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expected.Value, recorder.Value);
        Assert.True(recorder.ValueRecorded);
        Assert.Equal(expected.ValueLocation, recorder.ValueLocation);
    }

    [AssertionMethod]
    private static async Task FalseAndNotRecorded(ISyntacticAttributeParser parser, string source)
    {
        SyntacticConstructorSingleAttributeRecorder recorder = new();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);

        Assert.False(recorder.ValueRecorded);
    }

    private sealed class ExpectedResult
    {
        public object? Value { get; init; }
        public Location? ValueLocation { get; init; }
    }
}
