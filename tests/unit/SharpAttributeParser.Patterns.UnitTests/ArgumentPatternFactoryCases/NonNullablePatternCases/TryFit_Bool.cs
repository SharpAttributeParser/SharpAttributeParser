namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Bool
{
    private OneOf<Error, bool> Target(object? argument) => Pattern.TryFit(argument);

    private readonly IArgumentPattern<bool> Pattern = ((IArgumentPatternFactory)FactoryContext.Create().Factory).Bool();

    [Fact]
    public void Bool_True_ResultsInMatch() => ResultsInMatch(true, true);

    [Fact]
    public void Bool_False_ResultsInMatch() => ResultsInMatch(false, false);

    [Fact]
    public void Int_ResultsInError() => ResultsInError(0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(bool expected, object? argument)
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
