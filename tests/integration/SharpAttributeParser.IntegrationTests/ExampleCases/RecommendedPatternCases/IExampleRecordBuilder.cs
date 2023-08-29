namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

public interface IExampleRecordBuilder : IRecordBuilder<IExampleRecord>
{
    public abstract void WithTypeArgument(ITypeSymbol typeArgument);
    public abstract void WithConstructorArgument(StringComparison constructorArgument);
    public abstract void WithOptionalArgument(string? optionalArgument);
    public abstract void WithParamsArgument(IReadOnlyList<int> paramsArgument);
    public abstract void WithNamedArgument(ITypeSymbol? namedArgument);
}
