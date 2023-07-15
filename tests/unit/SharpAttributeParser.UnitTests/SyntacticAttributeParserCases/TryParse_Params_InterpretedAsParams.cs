namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Recording;

using System.Collections.Generic;
using System;
using System.Threading.Tasks;

using Xunit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

public sealed class TryParse_Params_InterpretedAsParams
{
    private ISyntacticParamsAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Params_InterpretedAsParams(ISyntacticParamsAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISyntacticAttributeParser parser, ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_CastedToDifferentType_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params((object?)null)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_CastedToDifferentType_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params((object?)default)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_DifferentType_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(default(object))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Empty_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => Array.Empty<ExpressionSyntax>();
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneParamsValue_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params("42")]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneArrayValuedParamsValue_ExplicitArray_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(new int[] { 4 })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneArrayValuedParamsValue_ImplicitArray_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(new[] { 4 })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneArrayValuedParamsValue_ExplicitlyEmptyArray_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(new int[0])]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task MultipleParamsValues_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params("42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<ExpressionSyntax> expected(AttributeSyntax attributeSyntax) => Enumerable.Range(0, 7).Select((index) => attributeSyntax.ArgumentList!.Arguments[index].Expression).ToList();
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(ISyntacticAttributeParser parser, string source, Func<AttributeSyntax, IReadOnlyList<ExpressionSyntax>> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(attributeSyntax);

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected, result.ValueSyntax.AsT1);
        Assert.True(result.ValueSyntaxRecorded);
    }
}
