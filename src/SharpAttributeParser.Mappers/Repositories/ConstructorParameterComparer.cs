namespace SharpAttributeParser.Mappers.Repositories;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IConstructorParameterComparer"/>
public sealed class ConstructorParameterComparer : IConstructorParameterComparer
{
    private IEqualityComparer<string> Name { get; }

    /// <summary>Instantiates a <see cref="ConstructorParameterComparer"/>, determining equality when comparing constructor parameters.</summary>
    /// <param name="name">Determines equality when comparing the names of constructor parameters.</param>
    public ConstructorParameterComparer(IEqualityComparer<string> name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    IEqualityComparer<string> IConstructorParameterComparer.Name => Name;
}
