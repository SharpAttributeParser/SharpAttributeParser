namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, OneOf<None, ExpressionSyntax>, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord>();

        var recorder = ((IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, OneOf<None, ExpressionSyntax>, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, OneOf<None, ExpressionSyntax>, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
