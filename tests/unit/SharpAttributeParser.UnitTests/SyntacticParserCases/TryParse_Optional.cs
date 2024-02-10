namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Optional
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArguments_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional((string)4, (string)5)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments;

            return new() { ValueASyntax = arguments[0].Expression, ValueBSyntax = arguments[1].Expression };
        }
    }

    [Fact]
    public async Task Empty_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueASyntax = new None(), ValueBSyntax = new None() };
    }

    [Fact]
    public async Task NullLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional(null, null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments;

            return new() { ValueASyntax = arguments[0].Expression, ValueBSyntax = arguments[1].Expression };
        }
    }

    [Fact]
    public async Task DefaultLiterasl_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional(default, default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments;

            return new() { ValueASyntax = arguments[0].Expression, ValueBSyntax = arguments[1].Expression };
        }
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional(default(object), default(object))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            var arguments = attributeSyntax.ArgumentList!.Arguments;

            return new() { ValueASyntax = arguments[0].Expression, ValueBSyntax = arguments[1].Expression };
        }
    }

    [Fact]
    public async Task OneUnlabelledValue_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional("42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueASyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression, ValueBSyntax = new None() };
    }

    [Fact]
    public async Task TwoUnlabelledValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional("42", "43")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (attributeArgument) => attributeArgument.Expression).ToList();

            return new() { ValueASyntax = arguments[0], ValueBSyntax = arguments[1] };
        }
    }

    [Fact]
    public async Task JustValueB_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional(valueB: "42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueASyntax = new None(), ValueBSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<AttributeSyntax, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(attributeSyntax);

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.True(outcome);

        expectedArguments.ValueASyntax.Switch
        (
            (none) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.Default.TryRecordArgument(constructorParameters[0]), Times.Once),
            (syntax) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.Normal.TryRecordArgument(constructorParameters[0], syntax), Times.Once)
        );

        expectedArguments.ValueBSyntax.Switch
        (
            (none) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.Default.TryRecordArgument(constructorParameters[1]), Times.Once),
            (syntax) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.Normal.TryRecordArgument(constructorParameters[1], syntax), Times.Once)
        );
    }

    private sealed class ExpectedArguments
    {
        public OneOf<None, ExpressionSyntax> ValueASyntax { get; init; }
        public OneOf<None, ExpressionSyntax> ValueBSyntax { get; init; }
    }
}
