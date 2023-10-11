namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Byte
{
    private OneOf<Error, byte> Target(object? argument) => Pattern.TryFit(argument);

    private readonly IArgumentPattern<byte> Pattern = ((IArgumentPatternFactory)FactoryContext.Create().Factory).Byte();

    [Fact]
    public void Byte_0_ResultsInMatch() => ResultsInMatch(0, (byte)0);

    [Fact]
    public void Byte_1_ResultsInMatch() => ResultsInMatch(1, (byte)1);

    [Fact]
    public void Int_ResultsInError() => ResultsInError(0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(byte expected, object? argument)
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
