namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

public interface IExampleRecord
{
    public ITypeSymbol TypeArgument { get; }
    public StringComparison ConstructorArgument { get; }
    public string? OptionalArgument { get; }
    public IReadOnlyList<int> ParamsArgument { get; }
    public ITypeSymbol? NamedArgument { get; }
}
