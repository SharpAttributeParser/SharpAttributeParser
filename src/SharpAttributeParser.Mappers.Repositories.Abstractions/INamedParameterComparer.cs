namespace SharpAttributeParser.Mappers.Repositories;

using System.Collections.Generic;

/// <summary>Determines equality when comparing named parameters.</summary>
public interface INamedParameterComparer
{
    /// <summary>Determines equality when comparing the names of named parameters.</summary>
    public abstract IEqualityComparer<string> Name { get; }
}
