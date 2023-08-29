namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Object
{
    private OneOf<Error, object?> Target(object? argument) => Pattern.TryFit(argument);

    private IArgumentPattern<object?> Pattern { get; } = ((IArgumentPatternFactory)FactoryContext.Create().Factory).NullableObject();

    [Fact]
    public void Object_ResultsInMatch()
    {
        var argument = Mock.Of<object>();

        ResultsInMatch(argument, argument);
    }

    [Fact]
    public void String_ResultsInMatch() => ResultsInMatch(string.Empty, string.Empty);

    [Fact]
    public void Int_ResultsInMatch() => ResultsInMatch(0, 0);

    [Fact]
    public void Null_ResultsInMatch() => ResultsInMatch(null, null);

    [AssertionMethod]
    private void ResultsInMatch(object? expected, object? argument)
    {
        var result = Target(argument);

        OneOfAssertions.Equal(expected, result);
    }
}
