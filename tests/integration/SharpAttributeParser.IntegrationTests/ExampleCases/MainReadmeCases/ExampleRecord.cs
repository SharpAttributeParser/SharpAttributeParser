namespace SharpAttributeParser.ExampleCases.MainReadmeCases;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

public sealed class ExampleRecord
{
    public ITypeSymbol? TypeArgument { get; set; }
    public StringComparison ConstructorArgument { get; set; }
    public string? OptionalArgument { get; set; }
    public IReadOnlyList<int>? ParamsArgument { get; set; }
    public ITypeSymbol? NamedArgument { get; set; }
}
