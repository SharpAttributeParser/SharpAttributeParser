namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticTypeArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, ExpressionSyntax, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>();

        var recorder = ((IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, ExpressionSyntax, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, ExpressionSyntax, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
