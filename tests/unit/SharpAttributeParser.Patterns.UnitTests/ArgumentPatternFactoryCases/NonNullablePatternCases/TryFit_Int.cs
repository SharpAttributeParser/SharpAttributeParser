namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Int
{
    private OneOf<Error, int> Target(object? argument) => Pattern.TryFit(argument);

    private IArgumentPattern<int> Pattern { get; } = ((IArgumentPatternFactory)FactoryContext.Create().Factory).Int();

    [Fact]
    public void Int_0_ResultsInMatch() => ResultsInMatch(0, 0);

    [Fact]
    public void Int_1_ResultsInMatch() => ResultsInMatch(1, 1);

    [Fact]
    public void Int_Negative1_ResultsInMatch() => ResultsInMatch(-1, -1);

    [Fact]
    public void Short_ResultsInError() => ResultsInError((short)0);

    [Fact]
    public void Long_ResultsInError() => ResultsInError((long)0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(int expected, object? argument)
    {
        var result = Target(argument);

        OneOfAssertions.Equal(expected, result);
    }

    [AssertionMethod]
    private void ResultsInError(object? argument)
    {
        var result = Target(argument);

        OneOfAssertions.Equal(new Error(), result);
    }
}
