namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_SimpleConstructor
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArguments_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor((string)4, (string)5)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new(arguments[0], arguments[1]);
        }
    }

    [Fact]
    public async Task NullLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(null, null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new(arguments[0], arguments[1]);
        }
    }

    [Fact]
    public async Task DefaultLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(default, default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new(arguments[0], arguments[1]);
        }
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(default(object), default(object))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new(arguments[0], arguments[1]);
        }
    }

    [Fact]
    public async Task Values_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor("42", "43")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new(arguments[0], arguments[1]);
        }
    }

    [Fact]
    public async Task ReverseOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [SimpleConstructor(valueB: 43, valueA: 42)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new(arguments[1], arguments[0]);
        }
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Constructor.Normal.TryRecordArgument(constructorParameters[0], expectedArguments.ValueASyntax), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Constructor.Normal.TryRecordArgument(constructorParameters[1], expectedArguments.ValueBSyntax), Times.Once);
    }

    private sealed class ExpectedArguments
    {
        public ExpressionSyntax ValueASyntax { get; }
        public ExpressionSyntax ValueBSyntax { get; }

        public ExpectedArguments(ExpressionSyntax valueASyntax, ExpressionSyntax valueBSyntax)
        {
            ValueASyntax = valueASyntax;
            ValueBSyntax = valueBSyntax;
        }
    }
}
