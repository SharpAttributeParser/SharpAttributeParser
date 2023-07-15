namespace SharpAttributeParser.AttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_NamedSimple
{
    private INamedAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_NamedSimple(INamedAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(IAttributeParser parser, IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(IAttributeParser parser)
    {
        var recorder = Mock.Of<IAttributeRecorder>(static (recorder) => recorder.TryRecordNamedArgument(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<ExpressionSyntax>()) == false);

        var source = """
            [Named(SimpleValue = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
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
            [Named(4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_TrueAndNotRecorded(IAttributeParser parser)
    {
        var source = """
            [Named(SimpleValue = (string)4)]
            public class Foo { }
            """;

        await TrueAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Named(SimpleValue = null)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Named(SimpleValue = default)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Named(SimpleValue = default(object[]))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Value_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Named(SimpleValue = "42")]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new("42", attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(IAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected.Value, result.SimpleValue);
        Assert.Equal(expected.ValueSyntax, result.SimpleValueSyntax);
        Assert.True(result.SimpleValueRecorded);
    }

    [AssertionMethod]
    private async Task ExpectedOutcomeAndNotRecorded(IAttributeParser parser, string source, bool expected)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.Equal(expected, outcome);

        Assert.False(result.SimpleValueRecorded);
    }

    [AssertionMethod]
    private async Task TrueAndNotRecorded(IAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, true);

    [AssertionMethod]
    private async Task FalseAndNotRecorded(IAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, false);

    private sealed class ExpectedResult
    {
        public object? Value { get; }
        public ExpressionSyntax ValueSyntax { get; }

        public ExpectedResult(object? value, ExpressionSyntax valueSyntax)
        {
            Value = value;
            ValueSyntax = valueSyntax;
        }
    }
}
