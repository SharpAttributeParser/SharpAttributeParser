namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_ArrayConstructor
{
    private ISyntacticArrayConstructorAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_ArrayConstructor(ISyntacticArrayConstructorAttributeRecorderFactory recorderFactory)
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
            [ArrayConstructor(null)]
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
            [ArrayConstructor(3, 4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ArrayConstructor((object[])4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(null)]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(default)]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(default(object[]))]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Empty_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(new object[] { })]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Values_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Labelled_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [ArrayConstructor(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [AssertionMethod]
    private async Task TrueAndIdenticalToExpected(ISyntacticAttributeParser parser, string source, Func<AttributeSyntax, ExpressionSyntax> expectedDelegate)
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
