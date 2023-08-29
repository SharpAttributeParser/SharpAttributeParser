namespace SharpAttributeParser.Patterns.ArgumentPatternFactoryCases;

using System;

using Xunit;

public sealed class NullableArray
{
    private IArgumentPattern<TElement[]?> Target<TElement>(IArgumentPattern<TElement> elementPattern) => ((IArgumentPatternFactory)Context.Factory).NullableArray(elementPattern);

    private FactoryContext Context { get; } = FactoryContext.Create();

    [Fact]
    public void Null_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target<object>(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }
}
