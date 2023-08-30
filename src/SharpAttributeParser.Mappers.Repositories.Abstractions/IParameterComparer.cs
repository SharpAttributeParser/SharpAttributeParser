namespace SharpAttributeParser.Mappers.Repositories;

/// <summary>Determines equality when comparing parameters.</summary>
public interface IParameterComparer
{
    /// <summary>Determines equality when comparing type parameters.</summary>
    public abstract ITypeParameterComparer TypeParameter { get; }

    /// <summary>Determines equality when comparing constructor parameters.</summary>
    public abstract IConstructorParameterComparer ConstructorParameter { get; }

    /// <summary>Determines equality when comparing named parameters.</summary>
    public abstract INamedParameterComparer NamedParameter { get; }
}
