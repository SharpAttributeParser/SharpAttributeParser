namespace SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="INamedParameterComparer"/>
public sealed class NamedParameterComparer : INamedParameterComparer
{
    private IEqualityComparer<string> Name { get; }

    /// <summary>Instantiates a <see cref="NamedParameterComparer"/>, determining equality when comparing named parameters.</summary>
    /// <param name="name">Determines equality when comparing the names of named parameters.</param>
    public NamedParameterComparer(IEqualityComparer<string> name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    IEqualityComparer<string> INamedParameterComparer.Name => Name;
}
