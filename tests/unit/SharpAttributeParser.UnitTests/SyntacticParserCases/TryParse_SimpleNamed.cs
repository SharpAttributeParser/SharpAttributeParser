namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_SimpleNamed
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArgument_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = (string)4)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task NullLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task DefaultLiteral_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task Value_ReturnsTrueAndRecords()
    {
        var source = """
            [Named(SimpleValue = "42")]
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

        Context.RecorderMock.Verify((recorder) => recorder.Named.TryRecordArgument(nameof(NamedAttribute.SimpleValue), expectedArguments.ValueSyntax!), Times.Exactly(expectedArguments.ValueSyntaxSetCount));
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
