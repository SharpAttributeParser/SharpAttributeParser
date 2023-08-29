namespace SharpAttributeParser.Mappers.Repositories.CombinedCases.DetachedMappedCombinedParamsConstructorArgumentRecorderFactoryCases.PatternedArgumentRecorderCases;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Patterns;

using System;
using System.Collections.Generic;

internal sealed class RecorderContext<TRecord, TArgument>
{
    public static RecorderContext<TRecord, TArgument> Create()
    {
        Mock<IArgumentPattern<TArgument>> patternMock = new();
        Mock<Func<TRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>> recorderDelegateMock = new();

        var recorderFactory = new DetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord>(Mock.Of<IArgumentPatternFactory>());

        var recorder = ((IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord>)recorderFactory).Create(patternMock.Object, recorderDelegateMock.Object);

        return new(recorder, patternMock, recorderDelegateMock);
    }

    public IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Recorder { get; }

    public Mock<IArgumentPattern<TArgument>> PatternMock { get; }
    public Mock<Func<TRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>> RecorderDelegateMock { get; }

    private RecorderContext(IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> recorder, Mock<IArgumentPattern<TArgument>> patternMock, Mock<Func<TRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool>> recorderDelegateMock)
    {
        Recorder = recorder;

        PatternMock = patternMock;
        RecorderDelegateMock = recorderDelegateMock;
    }
}
