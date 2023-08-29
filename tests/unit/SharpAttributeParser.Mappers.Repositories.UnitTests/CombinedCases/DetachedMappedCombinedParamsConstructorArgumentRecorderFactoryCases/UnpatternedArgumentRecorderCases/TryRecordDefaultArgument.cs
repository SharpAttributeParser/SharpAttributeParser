namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedParamsConstructorArgumentRecorderFactoryCases.UnpatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System.Collections.Generic;

using Xunit;

public sealed class TryRecordDefaultArgument
{
    private static bool Target<TRecord>(IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> recorder, TRecord dataRecord, object? argument) => recorder.TryRecordDefaultArgument(dataRecord, argument);

    [Fact]
    public void NullDataRecord_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        var outcome = Target(context.Recorder, null!, Mock.Of<object>());

        Assert.False(outcome);
    }

    [Fact]
    public void TrueReturningDelegate_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<object?>(), It.IsAny<OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>>())).Returns(true);

        var outcome = Target(context.Recorder, Mock.Of<object>(), Mock.Of<object>());

        Assert.False(outcome);
    }
}
