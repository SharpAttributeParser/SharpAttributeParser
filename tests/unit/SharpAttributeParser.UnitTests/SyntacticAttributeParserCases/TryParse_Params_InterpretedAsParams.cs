namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params_InterpretedAsParams
{
    private static bool Target(ISyntacticAttributeParser parser, ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_CastedToDifferentType_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params((object?)null)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = new object?[] { null },
            ValueLocation = ExpectedLocation.ParamsArgument(syntax, 0, 1)
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_CastedToDifferentType_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params((object?)default)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = new object?[] { null },
            ValueLocation = ExpectedLocation.ParamsArgument(syntax, 0, 1)
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_DifferentType_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(default(object))]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = new object?[] { null },
            ValueLocation = ExpectedLocation.ParamsArgument(syntax, 0, 1)
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Empty_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = Array.Empty<object?>(),
            ValueLocation = CollectionLocation.None
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneParamsValue_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params("42")]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = new object?[] { "42" },
            ValueLocation = ExpectedLocation.ParamsArgument(syntax, 0, 1)
        };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task MultipleParamsValues_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params("42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new()
            {
                Value = new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } },
                ValueLocation = ExpectedLocation.ParamsArgument(syntax, 0, 7)
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneArrayValuedParamsValue_True_Recorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Params(new int[] { 4 })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax syntax) => new()
        {
            Value = new object?[] { new[] { 4 } },
            ValueLocation = ExpectedLocation.ParamsArgument(syntax, 0, 1)
        };
    }

    [AssertionMethod]
    private static async Task TrueAndIdenticalToExpected(ISyntacticAttributeParser parser, string source, Func<Compilation, AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        SyntacticParamsAttributeRecorder recorder = new();

        var (compilation, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation, attributeSyntax);

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.True(result);

        Assert.Equal(expected.Value, recorder.Value);
        Assert.True(recorder.ValueRecorded);
        Assert.Equal(expected.ValueLocation!.Collection, recorder.ValueLocation!.Collection);
        Assert.Equal(expected.ValueLocation.Elements, recorder.ValueLocation.Elements);
    }

    private sealed class ExpectedResult
    {
        public IReadOnlyList<object?>? Value { get; init; }
        public CollectionLocation? ValueLocation { get; init; }
    }
}
