namespace SharpAttributeParser.SemanticParserCases;

using Microsoft.CodeAnalysis;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Linq.Expressions;

using Xunit;

public sealed class TryParse_Semantic
{
    private bool Target(ISemanticRecorder recorder, AttributeData attributeData) => Context.Parser.TryParse(recorder, attributeData);

    private ParserContext Context { get; } = ParserContext.Create();

    [Fact]
    public void NullAttributeClass_ReturnsFalseAndLogs()
    {
        CustomAttributeData attributeData = new(null, Mock.Of<IMethodSymbol>());

        ReturnsFalseAndLogs(attributeData, static (logger) => logger.UndeterminedAttributeClass());
    }

    [Fact]
    public void ErrorAttributeClass_ReturnsFalseAndLogs()
    {
        CustomAttributeData attributeData = new(Mock.Of<INamedTypeSymbol>(static (symbol) => symbol.TypeKind == TypeKind.Error), Mock.Of<IMethodSymbol>());

        ReturnsFalseAndLogs(attributeData, static (logger) => logger.UnrecognizedAttributeClass());
    }

    [Fact]
    public void NullAttributeConstructor_ReturnsFalseAndLogs()
    {
        CustomAttributeData attributeData = new(Mock.Of<INamedTypeSymbol>(), null);

        ReturnsFalseAndLogs(attributeData, static (logger) => logger.UndeterminedTargetConstructor());
    }

    [AssertionMethod]
    private void ReturnsFalseAndLogs(AttributeData attributeData, Expression<Action<ISemanticParserLogger<SemanticParser>>> loggerExpression)
    {
        var outcome = Target(Context.Recorder, attributeData);

        Assert.False(outcome);

        Context.LoggerMock.Verify(loggerExpression, Times.Once);
    }
}
