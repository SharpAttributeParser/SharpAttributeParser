namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases.NonNullablePatternCases;

using Microsoft.CodeAnalysis;

using Moq;

using OneOf;
using OneOf.Types;

using Xunit;

public sealed class TryFit_Type
{
    private OneOf<Error, ITypeSymbol> Target(object? argument) => Pattern.TryFit(argument);

    private IArgumentPattern<ITypeSymbol> Pattern { get; } = ((IArgumentPatternFactory)FactoryContext.Create().Factory).NonNullableType();

    [Fact]
    public void Type_ResultsInMatch()
    {
        var argument = Mock.Of<ITypeSymbol>();

        ResultsInMatch(argument, argument);
    }

    [Fact]
    public void Object_ResultsInError() => ResultsInError(Mock.Of<object>());

    [Fact]
    public void Null_ResultsInError() => ResultsInError(null);

    [AssertionMethod]
    private void ResultsInMatch(ITypeSymbol expected, object? argument)
    {
        var result = Target(argument);

        OneOfAssertions.Equal(OneOf<Error, ITypeSymbol>.FromT1(expected), result);
    }

    [AssertionMethod]
    private void ResultsInError(object? argument)
    {
        var result = Target(argument);

        OneOfAssertions.Equal(new Error(), result);
    }
}
