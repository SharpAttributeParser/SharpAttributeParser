namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_SimpleConstructorSimple
{
    private ISyntacticSimpleConstructorAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_SimpleConstructorSimple(ISyntacticSimpleConstructorAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISyntacticAttributeParser parser, ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticAttributeRecorder>(static (recorder) => recorder.TryRecordConstructorArgumentSyntax(It.IsAny<IParameterSymbol>(), It.IsAny<ExpressionSyntax>()) == false);

        var source = """
            [SimpleConstructor(null)]
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
            [SimpleConstructor(3, 4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [SimpleConstructor((string)4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [SimpleConstructor(null)]
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
            [SimpleConstructor(default)]
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
            [SimpleConstructor(default(object))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Value_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [SimpleConstructor("42")]
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

        Assert.Equal(expected, result.ValueSyntax);
        Assert.True(result.ValueSyntaxRecorded);
    }

    [AssertionMethod]
    private async Task FalseAndNotRecorded(ISyntacticAttributeParser parser, string source)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.False(outcome);

        Assert.False(result.ValueSyntaxRecorded);
    }
}
