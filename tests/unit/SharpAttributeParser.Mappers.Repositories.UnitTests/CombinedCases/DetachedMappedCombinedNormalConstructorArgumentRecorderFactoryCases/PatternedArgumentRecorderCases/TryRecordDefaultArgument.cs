﻿namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedNormalConstructorArgumentRecorderFactoryCases.PatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;

using Xunit;

public sealed class TryRecordDefaultArgument
{
    private static bool Target<TRecord, TArgument>(IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> recorder, TRecord dataRecord, TArgument argument) => recorder.TryRecordDefaultArgument(dataRecord, argument);

    [Fact]
    public void NullDataRecord_ReturnsFalse()
    {
        var context = RecorderContext<object, object>.Create();

        var outcome = Target(context.Recorder, null!, Mock.Of<object>());

        Assert.False(outcome);
    }

    [Fact]
    public void TrueReturningDelegate_ReturnsFalse()
    {
        var context = RecorderContext<object, object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<ExpressionSyntax>())).Returns(true);

        var outcome = Target(context.Recorder, Mock.Of<object>(), Mock.Of<object>());

        Assert.False(outcome);
    }
}
