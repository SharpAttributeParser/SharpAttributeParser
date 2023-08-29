namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticParamsConstructorArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System.Collections.Generic;

using Xunit;

public sealed class TryRecordDefaultArgument
{
    private static bool Target<TRecord>(IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> recorder, TRecord dataRecord) => recorder.TryRecordDefaultArgument(dataRecord);

    [Fact]
    public void NullDataRecord_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        var outcome = Target(context.Recorder, null!);

        Assert.False(outcome);
    }

    [Fact]
    public void NullSyntax_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        var outcome = Target(context.Recorder, Mock.Of<object>());

        Assert.False(outcome);
    }

    [Fact]
    public void TrueReturningDelegate_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>>())).Returns(true);

        var outcome = Target(context.Recorder, Mock.Of<object>());

        Assert.False(outcome);
    }
}
