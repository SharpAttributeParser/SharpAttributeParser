namespace SharpAttributeParser.Mappers.Repositories;

using System.Collections.Generic;

/// <summary>Determines equality when comparing constructor parameters.</summary>
public interface IConstructorParameterComparer
{
    /// <summary>Determines equality when comparing the names of constructor parameters.</summary>
    public abstract IEqualityComparer<string> Name { get; }
}
