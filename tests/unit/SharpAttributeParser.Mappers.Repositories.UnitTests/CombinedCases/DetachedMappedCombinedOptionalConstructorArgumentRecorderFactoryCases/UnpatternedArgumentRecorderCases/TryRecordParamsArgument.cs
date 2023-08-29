namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedOptionalConstructorArgumentRecorderFactoryCases.UnpatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System.Collections.Generic;

using Xunit;

public sealed class TryRecordParamsArgument
{
    private static bool Target<TRecord>(IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> recorder, TRecord dataRecord, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax) => recorder.TryRecordParamsArgument(dataRecord, argument, elementSyntax);

    [Fact]
    public void NullDataRecord_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        var outcome = Target(context.Recorder, null!, Mock.Of<object>(), Mock.Of<IReadOnlyList<ExpressionSyntax>>());

        Assert.False(outcome);
    }

    [Fact]
    public void NullSyntax_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        var outcome = Target(context.Recorder, Mock.Of<object>(), Mock.Of<object>(), null!);

        Assert.False(outcome);
    }

    [Fact]
    public void TrueReturningDelegate_ReturnsFalse()
    {
        var context = RecorderContext<object>.Create();

        context.RecorderDelegateMock.Setup(static (recorder) => recorder.Invoke(It.IsAny<object>(), It.IsAny<object?>(), It.IsAny<OneOf<None, ExpressionSyntax>>())).Returns(true);

        var outcome = Target(context.Recorder, Mock.Of<object>(), Mock.Of<object>(), Mock.Of<IReadOnlyList<ExpressionSyntax>>());

        Assert.False(outcome);
    }
}
