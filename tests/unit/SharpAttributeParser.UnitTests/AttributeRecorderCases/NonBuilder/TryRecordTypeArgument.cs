namespace SharpAttributeParser.AttributeRecorderCases.NonBuilder;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using System;

using Xunit;

public sealed class TryRecordTypeArgument
{
    private IAttributeRecorderFactory RecorderFactory { get; }

    public TryRecordTypeArgument(IAttributeRecorderFactory factory)
    {
        RecorderFactory = factory;
    }

    private static bool Target(IAttributeRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol argument, ExpressionSyntax syntax) => recorder.TryRecordTypeArgument(parameter, argument, syntax);

    [Fact]
    public void NullParameter_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, null!, Mock.Of<ITypeSymbol>(), ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullArgument_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, Mock.Of<ITypeParameterSymbol>(), null!, ExpressionSyntaxFactory.Create()));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullSyntax_ArgumentNullException()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var exception = Record.Exception(() => Target(recorder, Mock.Of<ITypeParameterSymbol>(), Mock.Of<ITypeSymbol>(), null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void NullReturningMapper_ReturnsFalse()
    {
        var recorder = RecorderFactory.Create(Mock.Of<IAttributeMapper<string>>(), string.Empty);

        var outcome = Target(recorder, Mock.Of<ITypeParameterSymbol>(), Mock.Of<ITypeSymbol>(), ExpressionSyntaxFactory.Create());

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

        var argument = Mock.Of<ITypeSymbol>();
        var syntax = ExpressionSyntaxFactory.Create();

        mapperMock.Setup(static (mapper) => mapper.TryMapTypeParameter(It.IsAny<ITypeParameterSymbol>(), It.IsAny<string>())).Returns(tryMapTypeParameter());

        var recorder = RecorderFactory.Create(mapperMock.Object, string.Empty);

        var outcome = Target(recorder, Mock.Of<ITypeParameterSymbol>(), argument, syntax);

        Assert.Equal(returnValue, outcome);

        Assert.Equal(argument, data.Argument);
        Assert.Equal(syntax, data.Syntax);

        IAttributeArgumentRecorder? tryMapTypeParameter() => new AttributeArgumentRecorder((argument, syntax) =>
        {
            data.Argument = (ITypeSymbol?)argument;
            data.Syntax = syntax.AsT0;

            return returnValue;
        });
    }

    private sealed class Data
    {
        public ITypeSymbol? Argument { get; set; }
        public ExpressionSyntax? Syntax { get; set; }
    }
}
