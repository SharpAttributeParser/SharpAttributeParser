namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedOptionalConstructorArgumentRecorderFactoryCases.UnpatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

using System;

internal sealed class RecorderContext<TRecord>
{
    public static RecorderContext<TRecord> Create()
    {
        Mock<Func<TRecord, object?, OneOf<None, ExpressionSyntax>, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>(Mock.Of<IArgumentPatternFactory>());

        var recorder = ((IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>)recorderFactory).Create(recorderDelegateMock.Object);

        return new(recorder, recorderDelegateMock);
    }

    public IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Recorder { get; }

    public Mock<Func<TRecord, object?, OneOf<None, ExpressionSyntax>, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> recorder, Mock<Func<TRecord, object?, OneOf<None, ExpressionSyntax>, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        RecorderDelegateMock = recorderDelegateMock;
    }
}
