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

public sealed class TryParse_OptionalParams
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorValues_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams((string)4, (string)5)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new() { ValueASyntax = arguments[0], ValueBSyntax = arguments[1] };
        }
    }

    [Fact]
    public async Task Empty_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueASyntax = new None(), ValueBSyntax = Array.Empty<ExpressionSyntax>() };
    }

    [Fact]
    public async Task NullLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(null, null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new() { ValueASyntax = arguments[0], ValueBSyntax = arguments[1] };
        }
    }

    [Fact]
    public async Task DefaultLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(default, default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new() { ValueASyntax = arguments[0], ValueBSyntax = arguments[1] };
        }
    }

    [Fact]
    public async Task DefaultExpressions_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(default(object), default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (argumentSyntax) => argumentSyntax.Expression).ToList();

            return new() { ValueASyntax = arguments[0], ValueBSyntax = arguments[1] };
        }
    }

    [Fact]
    public async Task OneUnlabelledValues_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams("42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueASyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression, ValueBSyntax = Array.Empty<ExpressionSyntax>() };
    }

    [Fact]
    public async Task TwoUnlabelledValues_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams("42", "43")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (attributeArgument) => attributeArgument.Expression).ToList();

            return new() { ValueASyntax = arguments[0], ValueBSyntax = new[] { arguments[1] } };
        }
    }

    [Fact]
    public async Task ThreeUnlabelledValues_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams("42", "43", "44")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (attributeArgument) => attributeArgument.Expression).ToList();

            return new() { ValueASyntax = arguments[0], ValueBSyntax = new[] { arguments[1], arguments[2] } };
        }
    }

    [Fact]

    public async Task JustValueB_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(valueB: new object[] { "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax) => new() { ValueASyntax = new None(), ValueBSyntax = attributeSyntax.ArgumentList!.Arguments[0].Expression };
    }

    [Fact]
    public async Task ReversedOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(valueB: new object[] { "43" }, valueA: "42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(AttributeSyntax attributeSyntax)
        {
            IReadOnlyList<ExpressionSyntax> arguments = attributeSyntax.ArgumentList!.Arguments.Select(static (attributeArgument) => attributeArgument.Expression).ToList();

            return new() { ValueASyntax = arguments[1], ValueBSyntax = arguments[0] };
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

        expectedArguments.ValueASyntax.Switch
        (
            (none) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordDefaultArgument(constructorParameters[0]), Times.Once),
            (syntax) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[0], syntax), Times.Once)
        );

        expectedArguments.ValueBSyntax.Switch
        (
            (syntax) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordArgument(constructorParameters[1], syntax), Times.Once),
            (elementSyntax) => Context.RecorderMock.Verify((recorder) => recorder.Constructor.TryRecordParamsArgument(constructorParameters[1], It.Is<IReadOnlyList<ExpressionSyntax>>(elementSyntax, SequenceEqualityComparer.Instance)), Times.Once)
        );
    }

    private sealed class ExpectedArguments
    {
        public OneOf<None, ExpressionSyntax> ValueASyntax { get; init; }
        public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueBSyntax { get; init; }
    }
}
