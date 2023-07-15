namespace SharpAttributeParser.AttributeParserCases;

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
    private IParamsAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Params_InterpretedAsParams(IParamsAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(IAttributeParser parser, IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_CastedToDifferentType_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params((object?)null)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(new object?[] { null }, new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_CastedToDifferentType_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params((object?)default)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(new object?[] { null }, new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_DifferentType_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(default(object))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(new object?[] { null }, new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Empty_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(Array.Empty<object?>(), Array.Empty<ExpressionSyntax>());
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneParamsValue_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params("42")]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(new object?[] { "42" }, new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneArrayValuedParamsValue_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(new int[] { 4 })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(new object?[] { new int[] { 4 } }, new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task MultipleParamsValues_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params("42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            var value = new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } };
            var syntax = attributeSyntax.ArgumentList!.Arguments.Select(static (argument) => argument.Expression).ToList();

            return new(value, syntax);
        }
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

        Assert.Equal(expected.Value, result.Value);
        Assert.Equal(expected.ValueSyntax, result.ValueSyntax.AsT1);
        Assert.True(result.ValueRecorded);
    }

    private sealed class ExpectedResult
    {
        public IReadOnlyList<object?>? Value { get; }
        public IReadOnlyList<ExpressionSyntax> ValueSyntax { get; }

        public ExpectedResult(IReadOnlyList<object?>? value, IReadOnlyList<ExpressionSyntax> valueSyntax)
        {
            Value = value;
            ValueSyntax = valueSyntax;
        }
    }
}
