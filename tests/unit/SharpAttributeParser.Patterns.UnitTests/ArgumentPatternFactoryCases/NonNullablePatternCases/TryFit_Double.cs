namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Double
{
    private OneOf<Error, double> Target(object? argument) => Pattern.TryFit(argument);

    private readonly IArgumentPattern<double> Pattern = ((IArgumentPatternFactory)FactoryContext.Create().Factory).Double();

    [Fact]
    public void Double_0_ResultsInMatch() => ResultsInMatch(0, 0d);

    [Fact]
    public void Double_Pi_ResultsInMatch() => ResultsInMatch(3.14, 3.14);

    [Fact]
    public void Double_NegativeE_ResultsInMatch() => ResultsInMatch(-2.718, -2.718);

    [Fact]
    public void Int_ResultsInError() => ResultsInError(0);

    [Fact]
    public void Float_ResultsInError() => ResultsInError(0f);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(double expected, object? argument)
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
