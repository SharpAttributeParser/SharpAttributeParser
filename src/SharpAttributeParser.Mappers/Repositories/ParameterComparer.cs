namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <inheritdoc cref="IParameterComparer"/>
public sealed class ParameterComparer : IParameterComparer
{
    private ITypeParameterComparer TypeParameter { get; }
    private IConstructorParameterComparer ConstructorParameter { get; }
    private INamedParameterComparer NamedParameter { get; }

    /// <summary>Instantiates a <see cref="ParameterComparer"/>, determining equality when comparing parameters.</summary>
    /// <param name="typeParameter">Determines equality when comparing type parameters.</param>
    /// <param name="constructorParameter">Determines equality when comparing constructor parameters.</param>
    /// <param name="namedParameter">Determines equality when comparing named parameters.</param>
    public ParameterComparer(ITypeParameterComparer typeParameter, IConstructorParameterComparer constructorParameter, INamedParameterComparer namedParameter)
    {
        TypeParameter = typeParameter ?? throw new ArgumentNullException(nameof(typeParameter));
        ConstructorParameter = constructorParameter ?? throw new ArgumentNullException(nameof(constructorParameter));
        NamedParameter = namedParameter ?? throw new ArgumentNullException(nameof(namedParameter));
    }

    ITypeParameterComparer IParameterComparer.TypeParameter => TypeParameter;
    IConstructorParameterComparer IParameterComparer.ConstructorParameter => ConstructorParameter;
    INamedParameterComparer IParameterComparer.NamedParameter => NamedParameter;
}
