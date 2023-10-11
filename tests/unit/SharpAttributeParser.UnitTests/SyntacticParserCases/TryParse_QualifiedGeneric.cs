namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_QualifiedGeneric
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task Qualified_ReturnsTrueAndRecords()
    {
        var source = """
            [SharpAttributeParser.QualifiedGeneric<string, int>]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)((QualifiedNameSyntax)attributeSyntax.Name).Right).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [Fact]
    public async Task AliasQualified_ReturnsTrueAndRecords()
    {
        var source = """
            using Alias = SharpAttributeParser;

            [Alias::QualifiedGeneric<string, int>]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var typeArguments = ((GenericNameSyntax)((AliasQualifiedNameSyntax)attributeSyntax.Name).Name).TypeArgumentList.Arguments;

            return new(typeArguments[0], typeArguments[1]);
        }
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var typeParameters = attributeData.AttributeClass!.TypeParameters;

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.TypeArgument.TryRecordArgument(typeParameters[0], expectedArguments.T1Syntax), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.TypeArgument.TryRecordArgument(typeParameters[1], expectedArguments.T2Syntax), Times.Once);
    }

    private sealed class ExpectedArguments
    {
        public ExpressionSyntax T1Syntax { get; }
        public ExpressionSyntax T2Syntax { get; }

        public ExpectedArguments(ExpressionSyntax t1syntax, ExpressionSyntax t2syntax)
        {
            T1Syntax = t1syntax;
            T2Syntax = t2syntax;
        }
    }
}
