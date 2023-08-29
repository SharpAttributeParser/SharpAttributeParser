namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_OptionalParams
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private ParserContext Context { get; } = ParserContext.Create();

    [Fact]
    public async Task ErrorOptionalArgument_ReturnsFalseAndLogs()
    {
        var source = """
            [Optional(valueA: (string)4)]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.ConstructorArgument.InvalidConstructorArgument());
    }

    [Fact]
    public async Task ErrorParamsArgument_ReturnsFalseAndLogs()
    {
        var source = """
            [Optional(valueB: (object?[])4)]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.ConstructorArgument.InvalidConstructorArgument());
    }

    [Fact]
    public async Task Empty_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, Array.Empty<object?>());
    }

    [Fact]
    public async Task NullLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(null, null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task DefaultLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(default, default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task DefaultExpressions_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(default(object), default(object[]))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task OneUnlabelledValues_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams("42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", Array.Empty<object?>());
    }

    [Fact]
    public async Task TwoUnlabelledValues_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams("42", "43")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", new[] { "43" });
    }

    [Fact]
    public async Task ThreeUnlabelledValues_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams("42", "43", "44")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", new[] { "43", "44" });
    }

    [Fact]
    public async Task JustValueB_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(valueB: new object[] { "42" })]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, new[] { "42" });
    }

    [Fact]
    public async Task ReversedOrder_ReturnsTrueAndRecords()
    {
        var source = """
            [OptionalParams(valueB: new object[] { "43" }, valueA: "42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", new[] { "43" });
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source, Func<Compilation, ExpectedArguments> expectedArgumentsDelegate)
    {
        var (compilation, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var constructorParameters = attributeData.AttributeConstructor!.Parameters;

        var expectedArguments = expectedArgumentsDelegate(compilation);

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordArgument(constructorParameters[0], It.Is(expectedArguments.ValueA, SequenceEqualityComparer.Instance)), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.ConstructorArgument.TryRecordArgument(constructorParameters[1], It.Is(expectedArguments.ValueB, SequenceEqualityComparer.Instance)), Times.Once);
    }

    [AssertionMethod]
    private async Task ReturnsFalseAndLogs(string source, Expression<Action<ISemanticParserLogger<SemanticParser>>> loggerExpression)
    {
        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(Context.Recorder, attributeData);

        Assert.False(outcome);

        Context.LoggerMock.Verify(loggerExpression, Times.Once);
    }

    private sealed class ExpectedArguments
    {
        public object? ValueA { get; }
        public object? ValueB { get; }

        public ExpectedArguments(object? valueA, object? valueB)
        {
            ValueA = valueA;
            ValueB = valueB;
        }
    }
}
