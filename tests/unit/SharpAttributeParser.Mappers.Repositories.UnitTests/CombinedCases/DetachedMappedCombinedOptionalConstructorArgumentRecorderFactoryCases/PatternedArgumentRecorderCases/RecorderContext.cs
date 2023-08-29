namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedOptionalConstructorArgumentRecorderFactoryCases.PatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;
using OneOf.Types;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

using System;

internal sealed class RecorderContext<TRecord, TArgument>
{
    public static RecorderContext<TRecord, TArgument> Create()
    {
        Mock<IArgumentPattern<TArgument>> patternMock = new();
        Mock<Func<TRecord, TArgument, OneOf<None, ExpressionSyntax>, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>(Mock.Of<IArgumentPatternFactory>());

        var recorder = ((IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<TRecord>)recorderFactory).Create(patternMock.Object, recorderDelegateMock.Object);

        return new(recorder, patternMock, recorderDelegateMock);
    }

    public IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Recorder { get; }

    public Mock<IArgumentPattern<TArgument>> PatternMock { get; }
    public Mock<Func<TRecord, TArgument, OneOf<None, ExpressionSyntax>, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> recorder, Mock<IArgumentPattern<TArgument>> patternMock, Mock<Func<TRecord, TArgument, OneOf<None, ExpressionSyntax>, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        PatternMock = patternMock;
        RecorderDelegateMock = recorderDelegateMock;
    }
}
