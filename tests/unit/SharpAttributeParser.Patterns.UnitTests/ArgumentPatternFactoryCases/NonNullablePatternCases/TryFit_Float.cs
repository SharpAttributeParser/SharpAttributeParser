namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Float
{
    private OneOf<Error, float> Target(object? argument) => Pattern.TryFit(argument);

    private IArgumentPattern<float> Pattern { get; } = ((IArgumentPatternFactory)FactoryContext.Create().Factory).Float();

    [Fact]
    public void Float_0_ResultsInMatch() => ResultsInMatch(0, 0f);

    [Fact]
    public void Float_Pi_ResultsInMatch() => ResultsInMatch(3.14f, 3.14f);

    [Fact]
    public void Float_NegativeE_ResultsInMatch() => ResultsInMatch(-2.718f, -2.718f);

    [Fact]
    public void Int_ResultsInError() => ResultsInError(0);

    [Fact]
    public void Double_ResultsInError() => ResultsInError(0d);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(float expected, object? argument)
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
