namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Combined
{
    private static bool Target(ISemanticAttributeParser parser, ISemanticArgumentRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Params_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, NamedSingleValue = typeof(int), NamedArrayValue = new object[] { 42, "42" })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new ExpectedResult(stringType, intType)
            {
                SingleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { "42", 42 },
                NamedSingleValue = intType,
                NamedArrayValue = new object[] { 42, "42" }
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyParams_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], NamedSingleValue = typeof(int), NamedArrayValue = new object[] { 42, "42" })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new ExpectedResult(stringType, intType)
            {
                SingleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = Array.Empty<object>(),
                NamedSingleValue = intType,
                NamedArrayValue = new object[] { 42, "42" }
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneParamsElement_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], 42, NamedSingleValue = typeof(int), NamedArrayValue = new object[] { 42, "42" })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new ExpectedResult(stringType, intType)
            {
                SingleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { 42 },
                NamedSingleValue = intType,
                NamedArrayValue = new object[] { 42, "42" }
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Array_True_Recorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, NamedSingleValue = typeof(int), NamedArrayValue = new object[] { 42, "42" })]
            public sealed class Foo { }
            """;

        await TrueAndIdenticalToExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new ExpectedResult(stringType, intType)
            {
                SingleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { "42", 42 },
                NamedSingleValue = intType,
                NamedArrayValue = new object[] { 42, "42" }
            };
        }
    }

    [AssertionMethod]
    private static async Task TrueAndIdenticalToExpected(ISemanticAttributeParser parser, string source, Func<Compilation, ExpectedResult> expectedDelegate)
    {
        SemanticCombinedAttributeRecorder recorder = new();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var result = Target(parser, recorder, attributeData);

        Assert.True(result);

        Assert.Equal(expected.T1, recorder.T1);
        Assert.True(recorder.T1Recorded);

        Assert.Equal(expected.T2, recorder.T2);
        Assert.True(recorder.T2Recorded);

        Assert.Equal(expected.SingleValue, recorder.SingleValue);
        Assert.True(recorder.SingleValueRecorded);

        Assert.Equal(expected.ArrayValue, recorder.ArrayValue);
        Assert.True(recorder.ArrayValueRecorded);

        Assert.Equal(expected.ParamsValue, recorder.ParamsValue);
        Assert.True(recorder.ParamsValueRecorded);

        Assert.Equal(expected.NamedSingleValue, recorder.NamedSingleValue);
        Assert.True(recorder.NamedSingleValueRecorded);

        Assert.Equal(expected.NamedArrayValue, recorder.NamedArrayValue);
        Assert.True(recorder.NamedArrayValueRecorded);
    }

    private sealed class ExpectedResult
    {
        public ITypeSymbol T1 { get; }
        public ITypeSymbol T2 { get; }

        public object? SingleValue { get; init; }
        public IReadOnlyList<object?>? ArrayValue { get; init; }
        public IReadOnlyList<object?>? ParamsValue { get; init; }
        public object? NamedSingleValue { get; init; }
        public IReadOnlyList<object?>? NamedArrayValue { get; init; }

        public ExpectedResult(ITypeSymbol t1, ITypeSymbol t2)
        {
            T1 = t1;
            T2 = t2;
        }
    }
}
