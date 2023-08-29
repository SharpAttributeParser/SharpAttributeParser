namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases;

using System;
using System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Class)]
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Used when parsing the attribute.")]
public sealed class ExampleAttribute<T> : Attribute
{
    public StringComparison ConstructorArgument { get; }
    public string? OptionalArgument { get; }
    public int[] ParamsArgument { get; }

    public Type? NamedArgument { get; init; }

    public ExampleAttribute(StringComparison constructorArgument, string? optionalArgument = null, params int[] paramsArgument)
    {
        ConstructorArgument = constructorArgument;
        OptionalArgument = optionalArgument;
        ParamsArgument = paramsArgument;
    }
}
