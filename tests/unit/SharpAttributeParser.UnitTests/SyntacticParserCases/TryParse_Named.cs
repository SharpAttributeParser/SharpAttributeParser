namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Named
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private ParserContext Context { get; } = ParserContext.Create();

    [Fact]
    public async Task ExpectedOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = default, ArrayValue = default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedResult expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new() { SimpleValueSyntax = arguments[0], ArrayValueSyntax = arguments[1] };
        }
    }

    [Fact]
    public async Task ReverseOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = default, SimpleValue = default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedResult expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new() { SimpleValueSyntax = arguments[1], ArrayValueSyntax = arguments[0] };
        }
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedResult> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.NamedArgument.TryRecordArgument(nameof(NamedAttribute.SimpleValue), expectedArguments.SimpleValueSyntax!), Times.Exactly(expectedArguments.SimpleValueSyntaxSetCount));
        Context.RecorderMock.Verify((recorder) => recorder.NamedArgument.TryRecordArgument(nameof(NamedAttribute.ArrayValue), expectedArguments.ArrayValueSyntax!), Times.Exactly(expectedArguments.ArrayValueSyntaxSetCount));
    }

    private sealed class ExpectedResult
    {
        public ExpressionSyntax? SimpleValueSyntax
        {
            get => RawSimpleValueSyntax;
            init
            {
                RawSimpleValueSyntax = value;
                SimpleValueSyntaxSet = true;
            }
        }

        public ExpressionSyntax? ArrayValueSyntax
        {
            get => RawArrayValueSyntax;
            init
            {
                RawArrayValueSyntax = value;
                ArrayValueSyntaxSet = true;
            }
        }

        public int SimpleValueSyntaxSetCount => SimpleValueSyntaxSet ? 1 : 0;
        public int ArrayValueSyntaxSetCount => ArrayValueSyntaxSet ? 1 : 0;

        private bool SimpleValueSyntaxSet { get; init; }
        private bool ArrayValueSyntaxSet { get; init; }

        private ExpressionSyntax? RawSimpleValueSyntax { get; init; }
        private ExpressionSyntax? RawArrayValueSyntax { get; init; }
    }
}
