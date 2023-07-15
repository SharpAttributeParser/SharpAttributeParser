namespace SharpAttributeParser.AttributeParserCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Recording;

using System.Collections.Generic;
using System;
using System.Threading.Tasks;

using Xunit;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public sealed class TryParse_Params_InterpretedAsArray
{
    private IParamsAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Params_InterpretedAsArray(IParamsAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(IAttributeParser parser, IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NullLiteral_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(null)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultLiteral_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(default)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task DefaultExpression_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(default(object[]))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(null, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ObjectArrayCastedToObject_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params((object)(new object[] { 4 }))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(new object?[] { 4 }, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task IntArrayCastedToObjectArray_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params((object[])(new int[] { 4 }))]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(new object?[] { 4 }, attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task PopulatedArray_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } }, attributeSyntax.ArgumentList!.Arguments[0].Expression);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyArray_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(new object[0])]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax) => new(Array.Empty<object?>(), attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Labelled_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Params(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            return new(new object?[] { "42", null, intType, "Foo", 42, (double)42, new object[] { "42", 42 } }, attributeSyntax.ArgumentList!.Arguments[0].Expression);
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
        Assert.Equal(expected.ValueSyntax, result.ValueSyntax);
        Assert.True(result.ValueRecorded);
    }

    private sealed class ExpectedResult
    {
        public IReadOnlyList<object?>? Value { get; }
        public ExpressionSyntax ValueSyntax { get; }

        public ExpectedResult(IReadOnlyList<object?>? value, ExpressionSyntax valueSyntax)
        {
            Value = value;
            ValueSyntax = valueSyntax;
        }
    }
}
