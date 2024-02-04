namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_ArrayConstructor
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArguments_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor((object[])4, (object[])5)]
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
            [ArrayConstructor(null, null)]
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
    public async Task DefaultLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(default, default)]
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
    public async Task DefaultExpressions_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(default(object[]), default(object[]))]
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
    public async Task EmptyArrays_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(new object[] { }, new object[] { })]
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
    public async Task PopulatedArrays_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(new object[] { "42", null, typeof(int), nameof(Foo) }, new object[] { ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
    public async Task Labelled_ReturnsTrueAndRecords()
    {
        var source = """
            [ArrayConstructor(valueA: new object[] { "42", null, typeof(int), nameof(Foo) }, valueB: new object[] { ((42)), (double)(float)42, new object[] { "42", 42 } })]
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
            [ArrayConstructor(valueB: new object?[] { 43 }, valueA: new object?[] { 42 })]
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

        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[0], expectedArguments.ValueASyntax), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[1], expectedArguments.ValueBSyntax), Times.Once);
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
