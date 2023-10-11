namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_SByte
{
    private OneOf<Error, sbyte> Target(object? argument) => Pattern.TryFit(argument);

    private readonly IArgumentPattern<sbyte> Pattern = ((IArgumentPatternFactory)FactoryContext.Create().Factory).SByte();

    [Fact]
    public void SByte_0_ResultsInMatch() => ResultsInMatch(0, (sbyte)0);

    [Fact]
    public void SByte_1_ResultsInMatch() => ResultsInMatch(1, (sbyte)1);

    [Fact]
    public void SByte_Negative1_ResultsInMatch() => ResultsInMatch(-1, (sbyte)-1);

    [Fact]
    public void Byte_ResultsInError() => ResultsInError((byte)0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(sbyte expected, object? argument)
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
