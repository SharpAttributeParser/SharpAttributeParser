namespace SharpAttributeParser.SyntacticAttributeRecorderCases.Builder;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;

using Xunit;

public sealed class TryRecordNamedArgumentSyntax
{
    private ISyntacticAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordNamedArgumentSyntax(ISyntacticAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(ISyntacticAttributeRecorder recorder, string parameterName, ExpressionSyntax syntax) => recorder.TryRecordNamedArgumentSyntax(parameterName, syntax);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(Mock.Of<ISyntacticAttributeMapper<IRecordBuilder<string>>>(), Mock.Of<IRecordBuilder<string>>());

        var exception = Record.Exception(() => Target(recorder, null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(Mock.Of<ISyntacticAttributeMapper<IRecordBuilder<string>>>(), Mock.Of<IRecordBuilder<string>>());

        var exception = Record.Exception(() => Target(recorder, string.Empty, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create<string, IRecordBuilder<string>>(Mock.Of<ISyntacticAttributeMapper<IRecordBuilder<string>>>(), Mock.Of<IRecordBuilder<string>>());

        var outcome = Target(recorder, string.Empty, ExpressionSyntaxFactory.Create());

        Assert.False(outcome);
    }

    [Fact]
    public void FalseReturningRecorderDelegate_UsesProvidedDelegateAndReturnsFalse() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(false);

    [Fact]
    public void DelegateReturningMapper_UsesProvidedDelegateAndReturnsTrue() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(true);

    [AssertionMethod]
    private void DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(bool returnValue)
    {
        Mock<ISyntacticAttributeMapper<IRecordBuilder<string>>> mapperMock = new();

        Data data = new();

        var syntax = ExpressionSyntaxFactory.Create();

        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<IRecordBuilder<string>>())).Returns(tryMapNamedParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, Mock.Of<IRecordBuilder<string>>());

        var outcome = Target(recorder, string.Empty, syntax);

        Assert.Equal(returnValue, outcome);

        Assert.Equal(syntax, data.Syntax);

        ISyntacticAttributeArgumentRecorder? tryMapNamedParameter() => new SyntacticAttributeArgumentRecorder((syntax) =>
        {
            data.Syntax = syntax.AsT0;

            return returnValue;
        });
    }

    private sealed class Data
    {
        public ExpressionSyntax? Syntax { get; set; }
    }
}
