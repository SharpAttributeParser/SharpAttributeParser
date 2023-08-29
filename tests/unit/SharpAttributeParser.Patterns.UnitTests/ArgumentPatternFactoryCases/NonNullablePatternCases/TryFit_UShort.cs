namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_UShort
{
    private OneOf<Error, ushort> Target(object? argument) => Pattern.TryFit(argument);

    private IArgumentPattern<ushort> Pattern { get; } = ((IArgumentPatternFactory)FactoryContext.Create().Factory).UShort();

    [Fact]
    public void UShort_0_ResultsInMatch() => ResultsInMatch(0, (ushort)0);

    [Fact]
    public void UShort_1_ResultsInMatch() => ResultsInMatch(1, (ushort)1);

    [Fact]
    public void Byte_ResultsInError() => ResultsInError((byte)0);

    [Fact]
    public void Int_ResultsInError() => ResultsInError(0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(ushort expected, object? argument)
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
