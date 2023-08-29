namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticNamedArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, ExpressionSyntax, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>();

        var recorder = ((IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, ExpressionSyntax, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, ExpressionSyntax, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
