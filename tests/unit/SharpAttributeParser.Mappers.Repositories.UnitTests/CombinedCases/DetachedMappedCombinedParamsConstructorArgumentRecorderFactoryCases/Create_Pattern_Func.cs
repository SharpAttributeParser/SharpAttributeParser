namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedParamsConstructorArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class Create_Pattern_Func
{
    private static IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Target<TRecord, TArgument>(IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord> factory, IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) => factory.Create(pattern, recorder);

    [Fact]
    public void NullPattern_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target<object, object>(context.Factory, null!, static (dataRecord, argument, syntax) => true));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, Mock.Of<IArgumentPattern<object>>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidPatternAndRecorder_ConstructedRecorderUsesPatternAndDelegate()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        var fitArgument = Mock.Of<object>();

        var context = FactoryContext<object>.Create();

        Mock<IArgumentPattern<object>> patternMock = new();
        Mock<Func<object, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>> recorderMock = new();

        patternMock.Setup(static (pattern) => pattern.TryFit(It.IsAny<object?>())).Returns(fitArgument);

        var recorder = Target(context.Factory, patternMock.Object, recorderMock.Object);

        recorder.TryRecordArgument(dataRecord, argument, syntax);

        patternMock.Verify((pattern) => pattern.TryFit(argument), Times.Once);
        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, fitArgument, syntax), Times.Once);
    }
}
