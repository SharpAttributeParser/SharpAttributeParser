namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedTypeArgumentRecorderFactoryCases.ArgumentRecorderCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, ITypeSymbol, ExpressionSyntax, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>();

        var recorder = ((IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedCombinedTypeArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, ITypeSymbol, ExpressionSyntax, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedCombinedTypeArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, ITypeSymbol, ExpressionSyntax, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
