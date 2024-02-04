namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_ArrayNamed
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArgument_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = (object[])4)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task NullLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task DefaultLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task EmptyArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = new object[0])]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task PopulatedArray_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(ArrayValue = new object[] { "42", null, typeof(int), nameof(Foo), ((42)), (double)(float)42, new object[] { "42", 42 } })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.Named.TryRecordArgument(nameof(NamedAttribute.ArrayValue), expectedArguments.ValueSyntax!), Times.Exactly(expectedArguments.ValueSyntaxSetCount));
    }

    private sealed class ExpectedArguments
    {
        public ExpressionSyntax? ValueSyntax
        {
            get => RawValueSyntax;
            init
            {
                RawValueSyntax = value;
                ValueSyntaxSet = true;
            }
        }

        public int ValueSyntaxSetCount => ValueSyntaxSet ? 1 : 0;

        private readonly bool ValueSyntaxSet;

        private readonly ExpressionSyntax? RawValueSyntax;
    }
}
