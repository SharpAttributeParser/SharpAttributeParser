namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(null!, Mock.Of<AttributeData>()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullAttributeData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(Context.Recorder, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public async Task FalseReturningTypeArgumentRecorder_ReturnsFalse()
    {
        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        await FalseReturningRecorder_ReturnsFalse(source);
    }

    [Fact]
    public async Task FalseReturningConstructorArgumentRecorder_ReturnsFalse()
    {
        var source = """
            [SimpleConstructor(42, "42")]
            public class Foo { }
            """;

        await FalseReturningRecorder_ReturnsFalse(source);
    }

    [Fact]
    public async Task FalseReturningNamedArgumentRecorder_ReturnsFalse()
    {
        var source = """
            [Named(SimpleValue = 42)]
            public class Foo { }
            """;

        await FalseReturningRecorder_ReturnsFalse(source);
    }

    [Fact]
    public async Task NonExistingAttribute_ReturnsFalseAndLogs()
    {
        var source = """
            [NonExisting]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.UnrecognizedAttributeClass());
    }

    [Fact]
    public async Task NonExistingConstructor_ReturnsFalseAndLogs()
    {
        var source = """
            [Generic<string, int>(4)]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.UndeterminedTargetConstructor());
    }

    [Fact]
    public async Task NonExistingNamedParameter_ReturnsTrue()
    {
        var source = """
            [Generic<string, int>(NonExisting = 4)]
            public class Foo { }
            """;

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);
    }

    [AssertionMethod]
    private async Task FalseReturningRecorder_ReturnsFalse(string source)
    {
        Mock<ISemanticRecorder> falseReturningRecorderMock = new();

        falseReturningRecorderMock.Setup(static (recorder) => recorder.TypeArgument.TryRecordArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ITypeSymbol>())).Returns(false);
        falseReturningRecorderMock.Setup(static (recorder) => recorder.ConstructorArgument.TryRecordArgument(It.IsAny<IParameterSymbol>(), It.IsAny<object?>())).Returns(false);
        falseReturningRecorderMock.Setup(static (recorder) => recorder.NamedArgument.TryRecordArgument(It.IsAny<string>(), It.IsAny<object?>())).Returns(false);

        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(falseReturningRecorderMock.Object, attributeData);

        Assert.False(result);
    }

    [AssertionMethod]
    private async Task ReturnsFalseAndLogs(string source, Expression<Action<ISemanticParserLogger<SemanticParser>>> loggerExpression)
    {
        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(Context.Recorder, attributeData);

        Assert.False(outcome);

        Context.LoggerMock.Verify(loggerExpression, Times.Once);
    }
}
