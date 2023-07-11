namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Generic
{
    private ISyntacticGenericAttributeRecorderFactory RecorderFactory { get; }

    public TryParse_Generic(ISyntacticGenericAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISyntacticAttributeParser parser, ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    [SuppressMessage("Minor Code Smell", "S1125: Boolean literals should not be redundant", Justification = "Prefer equality check over negation.")]
    public async Task FalseReturningRecorder_False(ISyntacticAttributeParser parser)
    {
        var recorder = Mock.Of<ISyntacticAttributeRecorder>(static (recorder) => recorder.TryRecordTypeArgumentSyntax(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ExpressionSyntax>()) == false);

        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(parser, recorder, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingAttribute_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExsitingConstructor_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task ErrorArgument_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<NonExisting, int>]
            public class Foo { }
            """;

        await FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task WithoutBody_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task WithBody_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>()]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Tuple_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<(string, int), (int, string)>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task Qualified_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [SharpAttributeParser.QualifiedGeneric<string, int>]
            public class Foo { }
            """;

        await TrueAndRecordedAsExpected(parser, source, expected);

        static ExpectedResult expected(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)((QualifiedNameSyntax)attributeSyntax.Name).Right).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
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
    }

    [AssertionMethod]
    private async Task FalseAndNotRecorded(ISyntacticAttributeParser parser, string source)
    {
        var recorder = RecorderFactory.Create();

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(parser, recorder, attributeData, attributeSyntax);
        var result = recorder.GetRecord();

        Assert.False(outcome);

        Assert.False(result.T1SyntaxRecorded);
        Assert.False(result.T2SyntaxRecorded);
    }

    private sealed class ExpectedResult
    {
        public ExpressionSyntax T1Syntax { get; }
        public ExpressionSyntax T2Syntax { get; }

        public ExpectedResult(ExpressionSyntax t1syntax, ExpressionSyntax t2syntax)
        {
            T1Syntax = t1syntax;
            T2Syntax = t2syntax;
        }
    }
}
