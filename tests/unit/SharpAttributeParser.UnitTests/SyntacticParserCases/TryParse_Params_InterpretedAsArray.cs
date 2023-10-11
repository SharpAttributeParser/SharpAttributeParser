namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params_InterpretedAsArray
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArgument_ReturnsTrueAndRecords()
    {
        var source = """
            [Params((object[])4)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Fact]
    public async Task NullLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Fact]
    public async Task DefaultLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Fact]
    public async Task ObjectArrayCastedToObject_ReturnsTrueAndRecords()
    {
        var source = """
            [Params((object)(new object[] { 4 }))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Fact]
    public async Task IntArrayCastedToObjectArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params((object[])(new int[] { 4 }))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Fact]
    public async Task PopulatedArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Fact]
    public async Task EmptyArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(new object[0])]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [Fact]
    public async Task Labelled_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(value: new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(attributeSyntax.ArgumentList!.Arguments[0].Expression);
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordArgument(constructorParameters[0], expectedArguments.Value), Times.Once);
    }

    private sealed class ExpectedArguments
    {
        public ExpressionSyntax Value { get; }

        public ExpectedArguments(ExpressionSyntax value)
        {
            Value = value;
        }
    }
}
