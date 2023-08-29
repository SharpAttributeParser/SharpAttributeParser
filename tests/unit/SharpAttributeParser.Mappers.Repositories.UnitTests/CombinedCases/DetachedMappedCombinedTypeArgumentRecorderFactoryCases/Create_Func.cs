namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedTypeArgumentRecorderFactoryCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

using Xunit;

public sealed class Create_Func
{
    private static IDetachedMappedCombinedTypeArgumentRecorder<TRecord> Target<TRecord>(IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord> factory, Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> recorder) => factory.Create(recorder);

    [Fact]
    public void NullRecorder_ArgumentNullException()
    {
        var context = FactoryContext<object>.Create();

        var exception = Record.Exception(() => Target(context.Factory, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidRecorder_ConstructedRecorderUsesDelegate()
    {
        var dataRecord = Mock.Of<object>();
        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        var context = FactoryContext<object>.Create();

        Mock<Func<object, ITypeSymbol, ExpressionSyntax, bool>> recorderMock = new();

        var recorder = Target(context.Factory, recorderMock.Object);

        recorder.TryRecordArgument(dataRecord, argument, syntax);

        recorderMock.Verify((recorder) => recorder.Invoke(dataRecord, argument, syntax), Times.Once);
    }
}
