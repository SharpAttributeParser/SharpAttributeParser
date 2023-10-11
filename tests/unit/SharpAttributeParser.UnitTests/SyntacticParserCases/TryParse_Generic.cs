namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Generic
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArgument_ReturnsTrueAndRecords()
    {
        var source = """
            [Generic<NonExisting, int>]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedResult expectedArguments(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [Fact]
    public async Task WithoutBody_ReturnsTrueAndRecords()
    {
        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedResult expectedArguments(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [Fact]
    public async Task WithBody_ReturnsTrueAndRecords()
    {
        var source = """
            [Generic<string, int>()]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedResult expectedArguments(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [Fact]
    public async Task Tuple_ReturnsTrueAndRecords()
    {
        var source = """
            [Generic<(string, int), (int, string)>]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedResult expectedArguments(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)attributeSyntax.Name).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedResult> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var typeParameters = attributeData.AttributeClass!.TypeParameters;

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.TypeArgument.TryRecordArgument(typeParameters[0], expectedArguments.T1Syntax), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.TypeArgument.TryRecordArgument(typeParameters[1], expectedArguments.T2Syntax), Times.Once);
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
