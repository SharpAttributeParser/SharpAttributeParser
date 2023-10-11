namespace SharpAttributeParser.SyntacticParserCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Logging;

using System;
using System.Linq.Expressions;

using Xunit;

public sealed class TryParse_Semantic
{
    private bool Target(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax) => Context.Parser.TryParse(recorder, attributeData, attributeSyntax);

    private readonly ParserContext Context = ParserContext.Create();

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
    private void ReturnsFalseAndLogs(AttributeData attributeData, Expression<Action<ISyntacticParserLogger<SyntacticParser>>> loggerExpression)
    {
        var outcome = Target(Context.Recorder, attributeData, AttributeSyntaxFactory.Create());

        Assert.False(outcome);

        Context.LoggerMock.Verify(loggerExpression, Times.Once);
    }
}
