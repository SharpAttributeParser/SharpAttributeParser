namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_UInt
{
    private OneOf<Error, uint> Target(object? argument) => Pattern.TryFit(argument);

    private readonly IArgumentPattern<uint> Pattern = ((IArgumentPatternFactory)FactoryContext.Create().Factory).UInt();

    [Fact]
    public void UInt_0_ResultsInMatch() => ResultsInMatch(0, (uint)0);

    [Fact]
    public void UInt_1_ResultsInMatch() => ResultsInMatch(1, (uint)1);

    [Fact]
    public void UShort_ResultsInError() => ResultsInError((ushort)0);

    [Fact]
    public void ULong_ResultsInError() => ResultsInError((ulong)0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(uint expected, object? argument)
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
