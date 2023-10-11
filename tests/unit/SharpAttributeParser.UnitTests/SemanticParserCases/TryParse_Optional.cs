namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Optional
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public async Task ErrorArguments_ReturnsFalseAndLogs()
    {
        var source = """
            [Optional((string)4, (string)5)]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.ConstructorArgument.InvalidConstructorArgument());
    }

    [Fact]
    public async Task Empty_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task NullLiterals_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional(null, null)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task DefaultLiterasl_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional(default, default)]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task DefaultExpression_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional(default(object), default(object))]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, null);
    }

    [Fact]
    public async Task OneUnlabelledValue_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional("42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", null);
    }

    [Fact]
    public async Task TwoUnlabelledValues_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional("42", "43")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new("42", "43");
    }

    [Fact]
    public async Task JustValueB_ReturnsTrueAndRecords()
    {
        var source = """
            [Optional(valueB: "42")]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source, expectedArguments);

        static ExpectedArguments expectedArguments(Compilation compilation) => new(null, "42");
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
