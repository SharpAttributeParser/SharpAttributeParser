namespace SharpAttributeParser.Mappers.Repositories.SyntacticCases.DetachedMappedSyntacticParamsConstructorArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;
using System.Collections.Generic;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord>();

        var recorder = ((IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
