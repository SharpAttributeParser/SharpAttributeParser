﻿namespace SharpAttributeParser.SyntacticAttributeRecorderCases.NonBuilder;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class TryRecordConstructorArgumentSyntax
{
    private ISyntacticAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordConstructorArgumentSyntax(ISyntacticAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(ISyntacticAttributeRecorder recorder, IParameterSymbol parameter, ExpressionSyntax syntax) => recorder.TryRecordConstructorArgumentSyntax(parameter, syntax);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISyntacticAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, null!, SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISyntacticAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, Mock.Of<IParameterSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create(Mock.Of<ISyntacticAttributeMapper<string>>(), string.Empty);

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));

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

        mapperMock.Setup(static (mapper) => mapper.TryMapConstructorParameter(It.IsAny<IParameterSymbol>(), It.IsAny<string>())).Returns(tryMapConstructorParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, string.Empty);

        var outcome = Target(recorder, Mock.Of<IParameterSymbol>(), syntax);

        Assert.Equal(returnValue, outcome);

        Assert.Equal(syntax, recordedSyntax, ReferenceEqualityComparer.Instance);

        ISyntacticAttributeConstructorArgumentRecorder? tryMapConstructorParameter() => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            recordedSyntax = syntax.AsT0;

            return returnValue;
        });
    }
}
