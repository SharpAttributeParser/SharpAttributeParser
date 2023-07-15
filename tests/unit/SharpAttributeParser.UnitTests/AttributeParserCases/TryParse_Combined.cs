namespace SharpAttributeParser.AttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser.Recording;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Combined
{
    private ICombinedAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Combined(ICombinedAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(IAttributeParser parser, IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ParamsWithNamed_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            IReadOnlyList<ExpressionSyntax> typeArgumentSyntaxes = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<AttributeArgumentSyntax> argumentSyntaxes = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(stringType, typeArgumentSyntaxes[0], intType, typeArgumentSyntaxes[1])
            {
                SimpleValue = "42",
                SimpleValueSyntax = argumentSyntaxes[0].Expression,
                ArrayValue = Array.Empty<object>(),
                ArrayValueSyntax = argumentSyntaxes[1].Expression,
                ParamsValue = new object[] { "42", 42 },
                ParamsValueSyntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(new[] { argumentSyntaxes[2].Expression, argumentSyntaxes[3].Expression }),
                SimpleNamedValue = intType,
                SimpleNamedValueSyntax = argumentSyntaxes[4].Expression,
                ArrayNamedValue = new object[] { 42, "42" },
                ArrayNamedValueSyntax = argumentSyntaxes[5].Expression
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ParamsWithoutNamed_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            IReadOnlyList<ExpressionSyntax> typeArgumentSyntaxes = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<AttributeArgumentSyntax> argumentSyntaxes = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(stringType, typeArgumentSyntaxes[0], intType, typeArgumentSyntaxes[1])
            {
                SimpleValue = "42",
                SimpleValueSyntax = argumentSyntaxes[0].Expression,
                ArrayValue = Array.Empty<object>(),
                ArrayValueSyntax = argumentSyntaxes[1].Expression,
                ParamsValue = new object[] { "42", 42 },
                ParamsValueSyntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(new[] { argumentSyntaxes[2].Expression, argumentSyntaxes[3].Expression })
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyParamsWithNamed_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            IReadOnlyList<ExpressionSyntax> typeArgumentSyntaxes = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<AttributeArgumentSyntax> argumentSyntaxes = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(stringType, typeArgumentSyntaxes[0], intType, typeArgumentSyntaxes[1])
            {
                SimpleValue = "42",
                SimpleValueSyntax = argumentSyntaxes[0].Expression,
                ArrayValue = Array.Empty<object>(),
                ArrayValueSyntax = argumentSyntaxes[1].Expression,
                ParamsValue = Array.Empty<object>(),
                ParamsValueSyntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(Array.Empty<ExpressionSyntax>()),
                SimpleNamedValue = intType,
                SimpleNamedValueSyntax = argumentSyntaxes[2].Expression,
                ArrayNamedValue = new object[] { 42, "42" },
                ArrayNamedValueSyntax = argumentSyntaxes[3].Expression
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyParamsWithoutNamed_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0])]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            IReadOnlyList<ExpressionSyntax> typeArgumentSyntaxes = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<AttributeArgumentSyntax> argumentSyntaxes = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(stringType, typeArgumentSyntaxes[0], intType, typeArgumentSyntaxes[1])
            {
                SimpleValue = "42",
                SimpleValueSyntax = argumentSyntaxes[0].Expression,
                ArrayValue = Array.Empty<object>(),
                ArrayValueSyntax = argumentSyntaxes[1].Expression,
                ParamsValue = Array.Empty<object>(),
                ParamsValueSyntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(Array.Empty<ExpressionSyntax>())
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneParamsElement_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            IReadOnlyList<ExpressionSyntax> typeArgumentSyntaxes = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<AttributeArgumentSyntax> argumentSyntaxes = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(stringType, typeArgumentSyntaxes[0], intType, typeArgumentSyntaxes[1])
            {
                SimpleValue = "42",
                SimpleValueSyntax = argumentSyntaxes[0].Expression,
                ArrayValue = Array.Empty<object>(),
                ArrayValueSyntax = argumentSyntaxes[1].Expression,
                ParamsValue = new object[] { 42 },
                ParamsValueSyntax = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(new[] { argumentSyntaxes[2].Expression }),
                SimpleNamedValue = intType,
                SimpleNamedValueSyntax = argumentSyntaxes[3].Expression,
                ArrayNamedValue = new object[] { 42, "42" },
                ArrayNamedValueSyntax = argumentSyntaxes[4].Expression
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ArrayWithNamed_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            IReadOnlyList<ExpressionSyntax> typeArgumentSyntaxes = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<AttributeArgumentSyntax> argumentSyntaxes = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(stringType, typeArgumentSyntaxes[0], intType, typeArgumentSyntaxes[1])
            {
                SimpleValue = "42",
                SimpleValueSyntax = argumentSyntaxes[0].Expression,
                ArrayValue = Array.Empty<object>(),
                ArrayValueSyntax = argumentSyntaxes[1].Expression,
                ParamsValue = new object[] { "42", 42 },
                ParamsValueSyntax = argumentSyntaxes[2].Expression,
                SimpleNamedValue = intType,
                SimpleNamedValueSyntax = argumentSyntaxes[3].Expression,
                ArrayNamedValue = new object[] { 42, "42" },
                ArrayNamedValueSyntax = argumentSyntaxes[4].Expression
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ArrayWithoutNamed_TrueAndRecorded(IAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(Compilation compilation, AttributeSyntax attributeSyntax)
        {
            var stringType = compilation.GetSpecialType(SpecialType.System_String);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);

            IReadOnlyList<ExpressionSyntax> typeArgumentSyntaxes = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            IReadOnlyList<AttributeArgumentSyntax> argumentSyntaxes = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(stringType, typeArgumentSyntaxes[0], intType, typeArgumentSyntaxes[1])
            {
                SimpleValue = "42",
                SimpleValueSyntax = argumentSyntaxes[0].Expression,
                ArrayValue = Array.Empty<object>(),
                ArrayValueSyntax = argumentSyntaxes[1].Expression,
                ParamsValue = new object[] { "42", 42 },
                ParamsValueSyntax = argumentSyntaxes[2].Expression
            };
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

        Assert.Equal(expected.T1, result.T1);
        Assert.Equal(expected.T1Syntax, result.T1Syntax);
        Assert.True(result.T1Recorded);

        Assert.Equal(expected.T2, result.T2);
        Assert.Equal(expected.T2Syntax, result.T2Syntax);
        Assert.True(result.T2Recorded);

        Assert.Equal(expected.SimpleValue, result.SimpleValue);
        Assert.Equal(expected.SimpleValueSyntax, result.SimpleValueSyntax);
        Assert.True(result.SimpleValueRecorded);

        Assert.Equal(expected.ArrayValue, result.ArrayValue);
        Assert.Equal(expected.ArrayValueSyntax, result.ArrayValueSyntax);
        Assert.True(result.ArrayValueRecorded);

        Assert.Equal(expected.ParamsValue, result.ParamsValue);
        OneOfAssertions.Equal(expected.ParamsValueSyntax, result.ParamsValueSyntax);
        Assert.True(result.ParamsValueRecorded);

        Assert.Equal(expected.SimpleNamedValue, result.SimpleNamedValue);
        Assert.Equal(expected.SimpleNamedValueSyntax, result.SimpleNamedValueSyntax);
        Assert.Equal(expected.SimpleNamedValue is not null, result.SimpleNamedValueRecorded);

        Assert.Equal(expected.ArrayNamedValue, result.ArrayNamedValue);
        Assert.Equal(expected.ArrayNamedValueSyntax, result.ArrayNamedValueSyntax);
        Assert.Equal(expected.ArrayNamedValue is not null, result.ArrayNamedValueRecorded);
    }

    private sealed class ExpectedResult
    {
        public ITypeSymbol T1 { get; }
        public ExpressionSyntax T1Syntax { get; }

        public ITypeSymbol T2 { get; }
        public ExpressionSyntax T2Syntax { get; }

        public object? SimpleValue { get; init; }
        public ExpressionSyntax? SimpleValueSyntax { get; init; }

        public IReadOnlyList<object?>? ArrayValue { get; init; }
        public ExpressionSyntax? ArrayValueSyntax { get; init; }

        public IReadOnlyList<object?>? ParamsValue { get; init; }
        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ParamsValueSyntax { get; init; }

        public object? SimpleNamedValue { get; init; }
        public ExpressionSyntax? SimpleNamedValueSyntax { get; init; }

        public IReadOnlyList<object?>? ArrayNamedValue { get; init; }
        public ExpressionSyntax? ArrayNamedValueSyntax { get; init; }

        public ExpectedResult(ITypeSymbol t1, ExpressionSyntax t1Syntax, ITypeSymbol t2, ExpressionSyntax t2Syntax)
        {
            T1 = t1;
            T1Syntax = t1Syntax;

            T2 = t2;
            T2Syntax = t2Syntax;
        }
    }
}
