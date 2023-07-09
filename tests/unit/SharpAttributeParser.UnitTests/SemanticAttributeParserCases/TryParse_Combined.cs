namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Recording;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Combined
{
    private ISemanticCombinedAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Combined(ISemanticCombinedAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISemanticAttributeParser parser, ISemanticAttributeRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Params_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new ExpectedResult(stringType, intType)
            {
                SimpleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { "42", 42 },
                SimpleNamedValue = intType,
                ArrayNamedValue = new object[] { 42, "42" }
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyParams_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new ExpectedResult(stringType, intType)
            {
                SimpleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = Array.Empty<object>(),
                SimpleNamedValue = intType,
                ArrayNamedValue = new object[] { 42, "42" }
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneParamsElement_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new ExpectedResult(stringType, intType)
            {
                SimpleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { 42 },
                SimpleNamedValue = intType,
                ArrayNamedValue = new object[] { 42, "42" }
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Array_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new ExpectedResult(stringType, intType)
            {
                SimpleValue = "42",
                ArrayValue = Array.Empty<object>(),
                ParamsValue = new object[] { "42", 42 },
                SimpleNamedValue = intType,
                ArrayNamedValue = new object[] { 42, "42" }
            };
        }
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(ISemanticAttributeParser parser, string source, Func<Compilation, ExpectedResult> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var outcome = Target(parser, recorder, attributeData);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected.T1, result.T1);
        Assert.True(result.T1Recorded);

        Assert.Equal(expected.T2, result.T2);
        Assert.True(result.T2Recorded);

        Assert.Equal(expected.SimpleValue, result.SimpleValue);
        Assert.True(result.SimpleValueRecorded);

        Assert.Equal(expected.ArrayValue, result.ArrayValue);
        Assert.True(result.ArrayValueRecorded);

        Assert.Equal(expected.ParamsValue, result.ParamsValue);
        Assert.True(result.ParamsValueRecorded);

        Assert.Equal(expected.SimpleNamedValue, result.SimpleNamedValue);
        Assert.True(result.SimpleNamedValueRecorded);

        Assert.Equal(expected.ArrayNamedValue, result.ArrayNamedValue);
        Assert.True(result.ArrayNamedValueRecorded);
    }

    private sealed class ExpectedResult
    {
        public ITypeSymbol T1 { get; }
        public ITypeSymbol T2 { get; }

        public object? SimpleValue { get; init; }
        public IReadOnlyList<object?>? ArrayValue { get; init; }
        public IReadOnlyList<object?>? ParamsValue { get; init; }
        public object? SimpleNamedValue { get; init; }
        public IReadOnlyList<object?>? ArrayNamedValue { get; init; }

        public ExpectedResult(ITypeSymbol t1, ITypeSymbol t2)
        {
            T1 = t1;
            T2 = t2;
        }
    }
}
