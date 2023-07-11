namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Recording;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params_InterpretedAsArray
{
    private ISyntacticParamsAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Params_InterpretedAsArray(ISyntacticParamsAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISyntacticAttributeParser parser, ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(null)]
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
            [Params(default)]
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
            [Params(default(object[]))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ObjectArrayCastedToObject_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params((object)(new object[] { 4 }))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task IntArrayCastedToObjectArray_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params((object[])(new int[] { 4 }))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task PopulatedArray_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
            [Params(new object[0])]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpressionSyntax expected(AttributeSyntax attributeSyntax) => attributeSyntax.ArgumentList!.Arguments[0].Expression;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Labelled_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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

        Assert.Equal(expected, result.ValueSyntax.AsT0);
        Assert.True(result.ValueSyntaxRecorded);
    }
}
