namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Short
{
    private OneOf<Error, short> Target(object? argument) => Pattern.TryFit(argument);

    private readonly IArgumentPattern<short> Pattern = ((IArgumentPatternFactory)FactoryContext.Create().Factory).Short();

    [Fact]
    public void Short_0_ResultsInMatch() => ResultsInMatch(0, (short)0);

    [Fact]
    public void Short_1_ResultsInMatch() => ResultsInMatch(1, (short)1);

    [Fact]
    public void Short_Negative1_ResultsInMatch() => ResultsInMatch(-1, (short)-1);

    [Fact]
    public void Byte_ResultsInError() => ResultsInError((byte)0);

    [Fact]
    public void Int_ResultsInError() => ResultsInError(0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(short expected, object? argument)
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
