namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_NamedArray
{
    private ISyntacticNamedAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_NamedArray(ISyntacticNamedAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISyntacticAttributeParser parser, ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticAttributeRecorder>(static (recorder) => recorder.TryRecordNamedArgumentSyntax(It.IsAny<string>(), It.IsAny<ExpressionSyntax>()) == false);

        var source = """
            [Named(ArrayValue = null)]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_TrueAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = (object[])4)]
            public class Foo { }
            """;

        await TrueAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = null)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = default)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = default(object[]))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyArray_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = new object[0])]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Values_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Named(ArrayValue = new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(ISyntacticAttributeParser parser, string source, Func<AttributeSyntax, ExpressionSyntax> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(attributeSyntax);

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected, result.ArrayValueSyntax);
        Assert.True(result.ArrayValueSyntaxRecorded);
    }

    [AssertionMethod]
    private async Task ExpectedOutcomeAndNotRecorded(ISyntacticAttributeParser parser, string source, bool expected)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.Equal(expected, outcome);

        Assert.False(result.ArrayValueSyntaxRecorded);
    }

    [AssertionMethod]
    private async Task TrueAndNotRecorded(ISyntacticAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, true);

    [AssertionMethod]
    private async Task FalseAndNotRecorded(ISyntacticAttributeParser parser, string source) => await ExpectedOutcomeAndNotRecorded(parser, source, false);
}
