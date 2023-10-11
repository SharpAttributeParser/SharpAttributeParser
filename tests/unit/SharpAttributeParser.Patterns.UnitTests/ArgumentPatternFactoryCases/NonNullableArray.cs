namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases;

using System;

using Xunit;

public sealed class NonNullableArray
{
    private IArgumentPattern<TElement[]> Target<TElement>(IArgumentPattern<TElement> elementPattern) => ((IArgumentPatternFactory)Context.Factory).NonNullableArray(elementPattern);

    private readonly FactoryContext Context = FactoryContext.Create();

    [Fact]
    public void Null_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target<object>(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }
}
