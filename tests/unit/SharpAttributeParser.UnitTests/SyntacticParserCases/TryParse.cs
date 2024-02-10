namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(null!, Mock.Of<AttributeData>(), AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullAttributeData_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(Context.Recorder, null!, AttributeSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullAttributeSyntax_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(Context.Recorder, Mock.Of<AttributeData>(), null!));

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
    public async Task FalseReturningNormalConstructorArgumentRecorder_ReturnsFalse()
    {
        var source = """
            [SimpleConstructor(42, "42")]
            public class Foo { }
            """;

        await FalseReturningRecorder_ReturnsFalse(source);
    }

    [Fact]
    public async Task FalseReturningParamsConstructorArgumentRecorder_ReturnsFalse()
    {
        var source = """
            [Params]
            public class Foo { }
            """;

        await FalseReturningRecorder_ReturnsFalse(source);
    }

    [Fact]
    public async Task FalseReturningDefaultConstructorArgumentRecorder_ReturnsFalse()
    {
        var source = """
            [Optional]
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

    [AssertionMethod]
    private async Task FalseReturningRecorder_ReturnsFalse(string source)
    {
        Mock<ISyntacticRecorder> falseReturningRecorderMock = new();

        falseReturningRecorderMock.Setup(static (recorder) => recorder.Type.TryRecordArgument(It.IsAny<ITypeParameterSymbol>(), It.IsAny<ExpressionSyntax>())).Returns(false);
        falseReturningRecorderMock.Setup(static (recorder) => recorder.Constructor.Normal.TryRecordArgument(It.IsAny<IParameterSymbol>(), It.IsAny<ExpressionSyntax>())).Returns(false);
        falseReturningRecorderMock.Setup(static (recorder) => recorder.Constructor.Params.TryRecordArgument(It.IsAny<IParameterSymbol>(), It.IsAny<IReadOnlyList<ExpressionSyntax>>())).Returns(false);
        falseReturningRecorderMock.Setup(static (recorder) => recorder.Constructor.Default.TryRecordArgument(It.IsAny<IParameterSymbol>())).Returns(false);
        falseReturningRecorderMock.Setup(static (recorder) => recorder.Named.TryRecordArgument(It.IsAny<string>(), It.IsAny<ExpressionSyntax>())).Returns(false);

        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var result = Target(falseReturningRecorderMock.Object, attributeData, attributeSyntax);

        Assert.False(result);
    }

    [AssertionMethod]
    private async Task ReturnsFalseAndLogs(string source, Expression<Action<ISyntacticParserLogger<SyntacticParser>>> loggerExpression)
    {
        var (_, attributeData, attributeSyntax) = await CompilationStore.GetComponents(source, "Foo");

        var outcome = Target(Context.Recorder, attributeData, attributeSyntax);

        Assert.False(outcome);

        Context.LoggerMock.Verify(loggerExpression, Times.Once);
    }
}
