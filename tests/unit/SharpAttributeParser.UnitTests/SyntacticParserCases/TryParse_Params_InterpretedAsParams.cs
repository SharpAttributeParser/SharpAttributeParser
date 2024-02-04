namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Params_InterpretedAsParams
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task NullLiteral_CastedToDifferentType_ReturnsTrueAndRecords()
    {
        var source = """
            [Params((object?)null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Fact]
    public async Task DefaultLiteral_CastedToDifferentType_ReturnsTrueAndRecords()
    {
        var source = """
            [Params((object?)default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Fact]
    public async Task DefaultExpression_DifferentType_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(default(object))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Fact]
    public async Task Empty_ReturnsTrueAndRecords()
    {
        var source = """
            [Params]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(Array.Empty<ExpressionSyntax>());
    }

    [Fact]
    public async Task OneParamsValue_ReturnsTrueAndRecords()
    {
        var source = """
            [Params("42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Fact]
    public async Task OneArrayValuedParamsValue_ExplicitArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(new int[] { 4 })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Fact]
    public async Task OneArrayValuedParamsValue_ImplicitArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(new[] { 4 })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Fact]
    public async Task OneArrayValuedParamsValue_ExplicitlyEmptyArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Params(new int[0])]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(new[] { attributeSyntax.ArgumentList!.Arguments[0].Expression });
    }

    [Fact]
    public async Task MultipleParamsValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Params("42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new(Enumerable.Range(0, 7).Select((index) => attributeSyntax.ArgumentList!.Arguments[index].Expression).ToList());
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordParamsArgument(constructorParameters[0], It.Is<IReadOnlyList<ExpressionSyntax>>(expectedArguments.Value, SequenceEqualityComparer.Instance)), Times.Once);
    }

    private sealed class ExpectedArguments
    {
        public IReadOnlyList<ExpressionSyntax> Value { get; }

        public ExpectedArguments(IReadOnlyList<ExpressionSyntax> value)
        {
            Value = value;
        }
    }
}
