namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Long
{
    private OneOf<Error, long> Target(object? argument) => Pattern.TryFit(argument);

    private IArgumentPattern<long> Pattern { get; } = ((IArgumentPatternFactory)FactoryContext.Create().Factory).Long();

    [Fact]
    public void Long_0_ResultsInMatch() => ResultsInMatch(0, (long)0);

    [Fact]
    public void Long_1_ResultsInMatch() => ResultsInMatch(1, (long)1);

    [Fact]
    public void Long_Negative1_ResultsInMatch() => ResultsInMatch(-1, (long)-1);

    [Fact]
    public void Int_ResultsInError() => ResultsInError(0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(long expected, object? argument)
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
