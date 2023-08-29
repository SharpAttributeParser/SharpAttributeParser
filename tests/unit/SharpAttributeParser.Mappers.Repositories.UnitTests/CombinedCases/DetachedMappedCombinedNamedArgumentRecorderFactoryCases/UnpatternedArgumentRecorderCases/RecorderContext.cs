namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedNamedArgumentRecorderFactoryCases.UnpatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

using System;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, object?, ExpressionSyntax, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>(Mock.Of<IArgumentPatternFactory>());

        var recorder = ((IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedCombinedNamedArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, object?, ExpressionSyntax, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedCombinedNamedArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, object?, ExpressionSyntax, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
