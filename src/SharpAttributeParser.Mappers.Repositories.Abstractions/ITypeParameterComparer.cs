namespace SharpAttributeParser.Mappers.Repositories;

using System.Collections.Generic;

/// <summary>Determines equality when comparing type parameters.</summary>
public interface ITypeParameterComparer
{
    /// <summary>Determines equality when comparing the names of type parameters.</summary>
    public abstract IEqualityComparer<string> Name { get; }
}
