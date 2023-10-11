namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_String
{
    private OneOf<Error, string?> Target(object? argument) => Pattern.TryFit(argument);

    private readonly IArgumentPattern<string?> Pattern = ((IArgumentPatternFactory)FactoryContext.Create().Factory).NullableString();

    [Fact]
    public void String_Empty_ResultsInMatch() => ResultsInMatch(string.Empty, string.Empty);

    [Fact]
    public void String_NonEmpty_ResultsInMatch() => ResultsInMatch("A", "A");

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Int_ResultsInError() => ResultsInError(0);

    [Fact]
    public void Null_ResultsInMatch() => ResultsInMatch(null, null);

    [AssertionMethod]
    private void ResultsInMatch(string? expected, object? argument)
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
