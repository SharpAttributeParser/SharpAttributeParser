namespace SharpAttributeParser.AttributeRecorderCases.NonBuilder;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;

using Xunit;

public sealed class TryRecordNamedArgument
{
    private IAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordNamedArgument(IAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(IAttributeRecorder recorder, string parameterName, object? argument, ExpressionSyntax syntax) => recorder.TryRecordNamedArgument(parameterName, argument, syntax);

    [Fact]
    public void NullParameterName_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, null!, null, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, string.Empty, null, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var outcome = Target(recorder, string.Empty, null, ExpressionSyntaxFactory.Create());

        Assert.False(outcome);
    }

    [Fact]
    public void FalseReturningRecorderDelegate_UsesProvidedDelegateAndReturnsFalse() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(false);

    [Fact]
    public void DelegateReturningMapper_UsesProvidedDelegateAndReturnsTrue() => DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(true);

    [AssertionMethod]
    private void DelegateReturningMapper_UsesProvidedDelegateAndPropagatesValue(bool returnValue)
    {
        Mock<IAttributeMapper<string>> mapperMock = new();

        Data data = new();

        var argument = Mock.Of<object>();
        var syntax = ExpressionSyntaxFactory.Create();

        mapperMock.Setup(static (mapper) => mapper.TryMapNamedParameter(It.IsAny<string>(), It.IsAny<string>())).Returns(tryMapNamedParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, string.Empty);

        var outcome = Target(recorder, string.Empty, argument, syntax);

        Assert.Equal(returnValue, outcome);

        Assert.Equal(argument, data.Argument);
        Assert.Equal(syntax, data.Syntax);

        IAttributeArgumentRecorder? tryMapNamedParameter() => new AttributeArgumentRecorder((argument, syntax) =>
        {
            data.Argument = argument;
            data.Syntax = syntax.AsT0;

            return returnValue;
        });
    }

    private sealed class Data
    {
        public object? Argument { get; set; }
        public ExpressionSyntax? Syntax { get; set; }
    }
}
