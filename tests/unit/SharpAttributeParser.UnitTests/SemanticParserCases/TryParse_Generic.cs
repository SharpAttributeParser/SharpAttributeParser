namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

public sealed class TryParse_Generic
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private ParserContext Context { get; } = ParserContext.Create();

    [Fact]
    public async Task ErrorArgument_ReturnsFalseAndLogs()
    {
        var source = """
            [Generic<NonExisting, int>]
            public class Foo { }
            """;

        await ReturnsFalseAndLogs(source, static (logger) => logger.TypeArgument.InvalidTypeArgument());
    }

    [Fact]
    public async Task WithoutBody_ReturnsTrueAndRecords()
    {
        var source = """
            [Generic<string, int>]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source);
    }

    [Fact]
    public async Task WithBody_ReturnsTrueAndRecords()
    {
        var source = """
            [Generic<string, int>()]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source);
    }

    [Fact]
    public async Task Tuple_ReturnsTrueAndRecords()
    {
        var source = """
            [Generic<(string, int), (int, string)>]
            public class Foo { }
            """;

        await ReturnsTrueAndRecords(source);
    }

    [AssertionMethod]
    private async Task ReturnsTrueAndRecords(string source)
    {
        var (_, attributeData, _) = await CompilationStore.GetComponents(source, "Foo");

        var typeParameters = attributeData.AttributeClass!.TypeParameters;
        var typeArguments = attributeData.AttributeClass.TypeArguments;

        var outcome = Target(Context.Recorder, attributeData);

        Assert.True(outcome);

        Context.RecorderMock.Verify((recorder) => recorder.TypeArgument.TryRecordArgument(typeParameters[0], typeArguments[0]), Times.Once);
        Context.RecorderMock.Verify((recorder) => recorder.TypeArgument.TryRecordArgument(typeParameters[1], typeArguments[1]), Times.Once);
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
