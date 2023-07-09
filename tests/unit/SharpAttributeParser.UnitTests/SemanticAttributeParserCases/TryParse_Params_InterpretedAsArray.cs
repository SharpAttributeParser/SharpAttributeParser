namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Recording;

using System.Collections.Generic;
using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params_InterpretedAsArray
{
    private ISemanticParamsAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Params_InterpretedAsArray(ISemanticParamsAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISemanticAttributeParser parser, ISemanticAttributeRecorder recorder, AttributeData attributeData) => parser.TryParse(recorder, attributeData);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(null)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(default)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(default(object[]))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => null;
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ObjectArrayCastedToObject_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((object)(new object[] { 4 }))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { 4 };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task IntArrayCastedToObjectArray_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params((object[])(new int[] { 4 }))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => new object?[] { 4 };
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task PopulatedArray_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyArray_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(new object[0])]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation) => Array.Empty<object?>();
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Labelled_TrueAndRecorded(ISemanticAttributeParser parser)
    {
        var source = """
            [Params(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static IReadOnlyList<object?>? expected(Compilation compilation)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } };
        }
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(ISemanticAttributeParser parser, string source, Func<Compilation, IReadOnlyList<object?>?> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(compilation);

        var outcome = Target(parser, recorder, attributeData);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected, result.Value);
        Assert.True(result.ValueRecorded);
    }
}
