namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedNamedArgumentRecorderFactoryCases.PatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

using System;

internal sealed class RecorderContext<TRecord, TArgument>
{
    public static RecorderContext<TRecord, TArgument> Create()
    {
        Mock<IArgumentPattern<TArgument>> patternMock = new();
        Mock<Func<TRecord, TArgument, ExpressionSyntax, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>(Mock.Of<IArgumentPatternFactory>());

        var recorder = ((IDetachedMappedCombinedNamedArgumentRecorderFactory<TRecord>)recorderFactory).Create(patternMock.Object, recorderDelegateMock.Object);

        return new(recorder, patternMock, recorderDelegateMock);
    }

    public IDetachedMappedCombinedNamedArgumentRecorder<TRecord> Recorder { get; }

    public Mock<IArgumentPattern<TArgument>> PatternMock { get; }
    public Mock<Func<TRecord, TArgument, ExpressionSyntax, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedCombinedNamedArgumentRecorder<TRecord> recorder, Mock<IArgumentPattern<TArgument>> patternMock, Mock<Func<TRecord, TArgument, ExpressionSyntax, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        PatternMock = patternMock;
        RecorderDelegateMock = recorderDelegateMock;
    }
}
