namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Recording;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse
{
    private ISyntacticGenericAttributeRecorderFactory RecorderFactory { get; }

    public TryParse(ISyntacticGenericAttributeRecorderFactory recorderFactory)
    {
        RecorderFactory = recorderFactory;
    }

    private static bool Target(ISyntacticAttributeParser parser, ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => parser.TryParse(recorder, attributeData, attributeSyntax);

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullRecorder_ArgumentNullException(ISyntacticAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, null!, Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullAttributeData_ArgumentNullException(ISyntacticAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, RecorderFactory.Create(), null!, AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public void NullAttributeSyntax_ArgumentNullException(ISyntacticAttributeParser parser)
    {
        var exception = Record.Exception(() => Target(parser, RecorderFactory.Create(), Mock.Of<AttributeData>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingAttribute_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingConstructor_FalseAndNotRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(4)]
            public class Foo { }
            """;

        await Generic_FalseAndNotRecorded(parser, source);
    }

    [Theory]
    [ClassData(typeof(ParserSources))]
    public async Task NonExistingNamedParameter_TrueAndRecorded(ISyntacticAttributeParser parser)
    {
        var source = """
            [Generic<string, int>(NonExisting = 4)]
            public class Foo { }
            """;

        await Generic_TrueAndRecorded(parser, source, expected);

        static ExpectedResult expected(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> typeArgumentExpressions = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(typeArgumentExpressions[0], typeArgumentExpressions[1]);
        }
    }

    [AssertionMethod]
    private async Task Generic_TrueAndRecorded(ISyntacticAttributeParser parser, string source, Func<AttributeSyntax, ExpectedResult> expectedDelegate)
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
    private async Task Generic_FalseAndNotRecorded(ISyntacticAttributeParser parser, string source)
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
        public ExpressionSyntax? T1Syntax { get; }
        public ExpressionSyntax? T2Syntax { get; }

        public ExpectedResult(ExpressionSyntax t1syntax, ExpressionSyntax t2syntax)
        {
            T1Syntax = t1syntax;
            T2Syntax = t2syntax;
        }
    }
}
