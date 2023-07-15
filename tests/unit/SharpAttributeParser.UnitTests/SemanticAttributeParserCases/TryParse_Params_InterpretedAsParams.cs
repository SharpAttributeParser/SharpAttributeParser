namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params_InterpretedAsParams
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_CastedToDifferentType_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((object?)null)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { null };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_CastedToDifferentType_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((object?)default)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { null };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_DifferentType_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(default(object))]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { null };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Empty_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => Array.Empty<object?>();
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneParamsValue_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params("42")]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { "42" };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneArrayValuedParamsValue_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(new int[] { 4 })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { new int[] { 4 } };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task MultipleParamsValues_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params("42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } };
        }
    }

    [AssertionMethod]
    private static async Task TrueAndIdenticalToExpected(ISemanticAttributeParser parser, string source, Func<Compilation, IReadOnlyList<object?>?> expectedDelegate)
    {
        SemanticParamsAttributeRecorder recorder = new();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expected, recorder.Value);
        Assert.True(recorder.ValueRecorded);
    }
}
