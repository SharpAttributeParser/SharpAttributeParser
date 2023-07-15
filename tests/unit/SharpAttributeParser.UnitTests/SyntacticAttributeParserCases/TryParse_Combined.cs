namespace SharpAttributeParser.SyntacticAttributeParserCases;

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
    private ISyntacticCombinedAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Combined(ISyntacticCombinedAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISyntacticAttributeParser parser, ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ParamsWithNamed_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new ExpectedResult(typeArguments[0], typeArguments[1])
            {
                SimpleValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression,
                ArrayValueSyntax = attributeSyntax.ArgumentList.Arguments[1].Expression,
                ParamsValueSyntax = new[] { attributeSyntax.ArgumentList.Arguments[2].Expression, attributeSyntax.ArgumentList.Arguments[3].Expression },
                SimpleNamedValueSyntax = attributeSyntax.ArgumentList.Arguments[4].Expression,
                ArrayNamedValueSyntax = attributeSyntax.ArgumentList.Arguments[5].Expression
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ParamsWithoutNamed_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], "42", 42)]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new ExpectedResult(typeArguments[0], typeArguments[1])
            {
                SimpleValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression,
                ArrayValueSyntax = attributeSyntax.ArgumentList.Arguments[1].Expression,
                ParamsValueSyntax = new[] { attributeSyntax.ArgumentList.Arguments[2].Expression, attributeSyntax.ArgumentList.Arguments[3].Expression }
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyParamsWithNamed_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new ExpectedResult(typeArguments[0], typeArguments[1])
            {
                SimpleValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression,
                ArrayValueSyntax = attributeSyntax.ArgumentList.Arguments[1].Expression,
                ParamsValueSyntax = Array.Empty<ExpressionSyntax>(),
                SimpleNamedValueSyntax = attributeSyntax.ArgumentList.Arguments[2].Expression,
                ArrayNamedValueSyntax = attributeSyntax.ArgumentList.Arguments[3].Expression
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task EmptyParamsWithoutNamed_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0])]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new ExpectedResult(typeArguments[0], typeArguments[1])
            {
                SimpleValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression,
                ArrayValueSyntax = attributeSyntax.ArgumentList.Arguments[1].Expression,
                ParamsValueSyntax = Array.Empty<ExpressionSyntax>()
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneParamsElement_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], 42, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            var arguments = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(typeArguments[0], typeArguments[1])
            {
                SimpleValueSyntax = arguments[0].Expression,
                ArrayValueSyntax = arguments[1].Expression,
                ParamsValueSyntax = new[] { arguments[2].Expression },
                SimpleNamedValueSyntax = arguments[3].Expression,
                ArrayNamedValueSyntax = arguments[4].Expression
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task OneArrayValuedParamsElement_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], new int[0])]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            var arguments = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(typeArguments[0], typeArguments[1])
            {
                SimpleValueSyntax = arguments[0].Expression,
                ArrayValueSyntax = arguments[1].Expression,
                ParamsValueSyntax = new[] { arguments[2].Expression }
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ArrayWithNamed_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 }, SimpleNamedValue = typeof(int), ArrayNamedValue = new object[] { 42, "42" })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            var arguments = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(typeArguments[0], typeArguments[1])
            {
                SimpleValueSyntax = arguments[0].Expression,
                ArrayValueSyntax = arguments[1].Expression,
                ParamsValueSyntax = arguments[2].Expression,
                SimpleNamedValueSyntax = arguments[3].Expression,
                ArrayNamedValueSyntax = arguments[4].Expression
            };
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ArrayWithoutNamed_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Combined<string, int>("42", new object[0], new object[] { "42", 42 })]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expectedResult);

        static ExpectedResult expectedResult(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;
            var arguments = attributeSyntax.ArgumentList!.Arguments;

            return new ExpectedResult(typeArguments[0], typeArguments[1])
            {
                SimpleValueSyntax = arguments[0].Expression,
                ArrayValueSyntax = arguments[1].Expression,
                ParamsValueSyntax = arguments[2].Expression
            };
        }
    }

    [AssertionMethod]
    private async Task TrueAndRecordedAsExpected(ISyntacticAttributeParser parser, string source, Func<AttributeSyntax, ExpectedResult> expectedDelegate)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expected = expectedDelegate(attributeSyntax);

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.True(outcome);

        Assert.Equal(expected.T1Syntax, result.T1Syntax);
        Assert.True(result.T1SyntaxRecorded);

        Assert.Equal(expected.T2Syntax, result.T2Syntax);
        Assert.True(result.T2SyntaxRecorded);

        Assert.Equal(expected.SimpleValueSyntax, result.SimpleValueSyntax);
        Assert.True(result.SimpleValueSyntaxRecorded);

        Assert.Equal(expected.ArrayValueSyntax, result.ArrayValueSyntax);
        Assert.True(result.ArrayValueSyntaxRecorded);

        OneOfAssertions.Equal(expected.ParamsValueSyntax, result.ParamsValueSyntax);
        Assert.True(result.ParamsValueSyntaxRecorded);

        Assert.Equal(expected.SimpleNamedValueSyntax, result.SimpleNamedValueSyntax);
        Assert.Equal(result.SimpleNamedValueSyntax is not null, result.SimpleNamedValueSyntaxRecorded);

        Assert.Equal(expected.ArrayNamedValueSyntax, result.ArrayNamedValueSyntax);
        Assert.Equal(result.ArrayNamedValueSyntax is not null, result.ArrayNamedValueSyntaxRecorded);
    }

    private sealed class ExpectedResult
    {
        public ExpressionSyntax T1Syntax { get; }
        public ExpressionSyntax T2Syntax { get; }

        public ExpressionSyntax? SimpleValueSyntax { get; init; }
        public ExpressionSyntax? ArrayValueSyntax { get; init; }
        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ParamsValueSyntax { get; init; }
        public ExpressionSyntax? SimpleNamedValueSyntax { get; init; }
        public ExpressionSyntax? ArrayNamedValueSyntax { get; init; }

        public ExpectedResult(ExpressionSyntax t1syntax, ExpressionSyntax t2syntax)
        {
            T1Syntax = t1syntax;
            T2Syntax = t2syntax;
        }
    }
}
