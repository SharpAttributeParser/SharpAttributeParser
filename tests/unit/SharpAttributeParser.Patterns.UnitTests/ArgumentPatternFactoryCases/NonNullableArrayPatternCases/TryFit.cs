namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullableArrayPatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using System;

using Xunit;

public sealed class TryFit
{
    private static OneOf<Error, TElement[]> Target<TElement>(IArgumentPattern<TElement[]> pattern, object? argument) => pattern.TryFit(argument);

    private readonly FactoryContext Context = FactoryContext.Create();

    [Fact]
    public void Empty_ResultsInMatch() => ResultsInMatch(Mock.Of<IArgumentPattern<object>>(), Array.Empty<object>(), Array.Empty<object>());

    [Fact]
    public void ResultReturningElementPattern_UsesElementPatternAndResultsInMatch()
    {
        var inputArgument = Mock.Of<object>();
        var outputArgument = Mock.Of<object>();

        Mock<IArgumentPattern<object>> elementPatternMock = new();

        elementPatternMock.Setup(static (pattern) => pattern.TryFit(It.IsAny<object?>())).Returns(outputArgument);

        ResultsInMatch(elementPatternMock.Object, new[] { outputArgument, outputArgument }, new[] { inputArgument, inputArgument });

        elementPatternMock.Verify((pattern) => pattern.TryFit(inputArgument), Times.Exactly(2));
    }

    [Fact]
    public void ErrorReturningElementPattern_ResultsInError()
    {
        Mock<IArgumentPattern<object>> elementPatternMock = new();

        elementPatternMock.Setup(static (pattern) => pattern.TryFit(It.IsAny<object?>())).Returns(new Error());

        ResultsInError(elementPatternMock.Object, new[] { Mock.Of<object>() });
    }

    [Fact]
    public void Null_ResultsInError() => ResultsInError(Mock.Of<IArgumentPattern<object>>(), null);

    [AssertionMethod]
    private void ResultsInMatch<TElement>(IArgumentPattern<TElement> elementPattern, TElement[] expected, object? argument)
    {
        var pattern = ((IArgumentPatternFactory)Context.Factory).NonNullableArray(elementPattern);

        var result = Target(pattern, argument);

        OneOfAssertions.Equal(expected, result);
    }

    [AssertionMethod]
    private void ResultsInError<TElement>(IArgumentPattern<TElement> elementPattern, object? argument)
    {
        var pattern = ((IArgumentPatternFactory)Context.Factory).NonNullableArray(elementPattern);

        var result = Target(pattern, argument);

        OneOfAssertions.Equal(new Error(), result);
    }
}
