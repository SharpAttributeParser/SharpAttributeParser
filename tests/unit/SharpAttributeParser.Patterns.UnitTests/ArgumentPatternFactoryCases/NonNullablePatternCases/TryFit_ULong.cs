namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_ULong
{
    private OneOf<Error, ulong> Target(object? argument) => Pattern.TryFit(argument);

    private IArgumentPattern<ulong> Pattern { get; } = ((IArgumentPatternFactory)FactoryContext.Create().Factory).ULong();

    [Fact]
    public void ULong_0_ResultsInMatch() => ResultsInMatch(0, (ulong)0);

    [Fact]
    public void ULong_1_ResultsInMatch() => ResultsInMatch(1, (ulong)1);

    [Fact]
    public void UInt_ResultsInError() => ResultsInError((uint)0);

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(ulong expected, object? argument)
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
