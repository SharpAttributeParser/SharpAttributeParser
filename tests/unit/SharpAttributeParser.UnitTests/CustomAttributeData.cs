namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Collections.Immutable;

internal sealed class CustomAttributeData : AttributeData
{
    new private readonly INamedTypeSymbol? AttributeClass;
    new private readonly IMethodSymbol? AttributeConstructor;

    public CustomAttributeData(INamedTypeSymbol? attributeClass, IMethodSymbol? attributeConstructor)
    {
        AttributeClass = attributeClass;
        AttributeConstructor = attributeConstructor;
    }

    protected override INamedTypeSymbol? CommonAttributeClass => AttributeClass;
    protected override IMethodSymbol? CommonAttributeConstructor => AttributeConstructor;

    protected override SyntaxReference? CommonApplicationSyntaxReference => null;
    protected override ImmutableArray<TypedConstant> CommonConstructorArguments => new();
    protected override ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments => new();
}
