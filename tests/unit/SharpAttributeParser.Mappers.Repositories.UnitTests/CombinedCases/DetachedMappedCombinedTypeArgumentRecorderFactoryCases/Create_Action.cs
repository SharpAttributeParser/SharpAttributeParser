namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedTypeArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

using Xunit;

public sealed class Create_Action
{
    private static IDetachedMappedCombinedTypeArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord> factory, Action<TRecord, ITypeSymbol, ExpressionSyntax> recorder) => factory.Create(recorder);

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_ConstructedRecorderUsesDelegateAndReturnsTrue()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        var context = FactoryContext<object>.Create();

        Mock<Action<object, ITypeSymbol, ExpressionSyntax>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        var outcome = recorder.TryRecordArgument(dataRecord, argument, syntax);

        Assert.True(outcome);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, argument, syntax), Times.Once);
    }
}
