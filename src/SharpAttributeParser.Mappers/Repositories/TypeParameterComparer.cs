namespace SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ITypeParameterComparer"/>
public sealed class TypeParameterComparer : ITypeParameterComparer
{
    private readonly IEqualityComparer<string> Name;

    /// <summary>Instantiates a <see cref="TypeParameterComparer"/>, determining equality when comparing type parameters.</summary>
    /// <param name="name">Determines equality when comparing the names of type parameters.</param>
    public TypeParameterComparer(IEqualityComparer<string> name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    IEqualityComparer<string> ITypeParameterComparer.Name => Name;
}
