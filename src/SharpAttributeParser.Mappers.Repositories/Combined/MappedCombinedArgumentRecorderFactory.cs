namespace SharpAttributeParser.Mappers.Repositories.Combined;

using System;

/// <inheritdoc cref="IMappedCombinedArgumentRecorderFactory"/>
public sealed class MappedCombinedArgumentRecorderFactory : IMappedCombinedArgumentRecorderFactory
{
    private readonly IMappedCombinedTypeArgumentRecorderFactory TypeParameter;
    private readonly IMappedCombinedConstructorArgumentRecorderFactory ConstructorParameter;
    private readonly IMappedCombinedNamedArgumentRecorderFactory NamedParameter;

    /// <summary>Instantiates a <see cref="MappedCombinedArgumentRecorderFactory"/>, handling creation of attached recorders.</summary>
    /// <param name="typeParameter">Handles creation of attached recorders related to type parameters.</param>
    /// <param name="constructorParameter">Handles creation of attached recorders related to constructor parameters.</param>
    /// <param name="namedParameter">Handles creation of attached recorders related to named parameters.</param>
    public MappedCombinedArgumentRecorderFactory(IMappedCombinedTypeArgumentRecorderFactory typeParameter, IMappedCombinedConstructorArgumentRecorderFactory constructorParameter, IMappedCombinedNamedArgumentRecorderFactory namedParameter)
    {
        TypeParameter = typeParameter ?? throw new ArgumentNullException(nameof(typeParameter));
        ConstructorParameter = constructorParameter ?? throw new ArgumentNullException(nameof(constructorParameter));
        NamedParameter = namedParameter ?? throw new ArgumentNullException(nameof(namedParameter));
    }

    IMappedCombinedTypeArgumentRecorderFactory IMappedCombinedArgumentRecorderFactory.TypeParameter => TypeParameter;
    IMappedCombinedConstructorArgumentRecorderFactory IMappedCombinedArgumentRecorderFactory.ConstructorParameter => ConstructorParameter;
    IMappedCombinedNamedArgumentRecorderFactory IMappedCombinedArgumentRecorderFactory.NamedParameter => NamedParameter;
}
