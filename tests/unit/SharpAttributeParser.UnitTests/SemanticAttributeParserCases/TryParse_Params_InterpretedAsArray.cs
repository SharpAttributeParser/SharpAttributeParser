namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params_InterpretedAsArray
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(null)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(default)]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(default(object[]))]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ObjectArrayCastedToObject_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((object)(new object[] { 4 }))]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { 4 };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task IntArrayCastedToObjectArray_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((object[])(new int[] { 4 }))]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { 4 };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task PopulatedArray_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyArray_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(new object[0])]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => Array.Empty<object?>();
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Labelled_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
