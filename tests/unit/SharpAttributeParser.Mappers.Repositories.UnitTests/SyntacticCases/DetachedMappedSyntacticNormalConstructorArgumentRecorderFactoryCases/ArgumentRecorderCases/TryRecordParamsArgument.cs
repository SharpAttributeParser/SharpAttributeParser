namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticNormalConstructorArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System.Collections.Generic;

using Xunit;

public sealed class TryRecordParamsArgument
{
    private static bool Target<TRecord>(IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> recorder, TRecord dataRecord, IReadOnlyList<ExpressionSyntax> elementSyntax) => recorder.TryRecordParamsArgument(dataRecord, elementSyntax);

    [Fact]
    public void NullDataRecord_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        var outcome = Target(context.Recorder, null!, Mock.Of<IReadOnlyList<ExpressionSyntax>>());

        Assert.False(outcome);
    }

    [Fact]
    public void NullElementSyntax_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        var outcome = Target(context.Recorder, Mock.Of<object>(), null!);

        Assert.False(outcome);
    }

    [Fact]
    public void TrueReturningDelegate_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<ExpressionSyntax>())).Returns(true);

        var outcome = Target(context.Recorder, Mock.Of<object>(), Mock.Of<IReadOnlyList<ExpressionSyntax>>());

        Assert.False(outcome);
    }
}
