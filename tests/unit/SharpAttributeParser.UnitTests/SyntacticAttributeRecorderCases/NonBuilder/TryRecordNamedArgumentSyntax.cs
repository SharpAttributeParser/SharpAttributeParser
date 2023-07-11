namespace SharpAttributeParser.SyntacticAttributeRecorderCases.NonBuilder;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordNamedArgumentSyntax
{
    private ISyntacticAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordNamedArgumentSyntax(ISyntacticAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(ISyntacticAttributeRecorder recorder, string parameterName, ExpressionSyntax syntax) => recorder.TryRecordNamedArgumentSyntax(parameterName, syntax);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISyntacticAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, null!, SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISyntacticAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, string.Empty, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISyntacticAttributeMapper<string>>(), string.Empty);

        var outcome = Target(recorder, string.Empty, SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));

        Assert.False(outcome);
    }

    [Fact]
    public void FalseReturningRecorderDelegate_UsesProvidedDelegateAndReturnsFalse() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(false);

    [Fact]
    public void DelegateReturningMapper_UsesProvidedDelegateAndReturnsTrue() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(true);

    [AssertionMethod]
    private void DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(bool returnValue)
    {
        Mock<ISyntacticAttributeMapper<string>> mapperMock = new();

        ExpressionSyntax? recordedSyntax = null;
        var syntax = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<string>())).Returns(tryMapNamedParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, string.Empty);

        var outcome = Target(recorder, string.Empty, syntax);

        Assert.Equal(returnValue, outcome);

        Assert.Equal(syntax, recordedSyntax, ReferenceEqualityComparer.Instance);

        ISyntacticAttributeArgumentRecorder? tryMapNamedParameter() => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            recordedSyntax = syntax.AsT0;

            return returnValue;
        });
    }
}
