namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using System;

/// <inheritdoc cref="IMappedSemanticArgumentRecorderFactory"/>
public sealed class MappedSemanticArgumentRecorderFactory : IMappedSemanticArgumentRecorderFactory
{
    private IMappedSemanticTypeArgumentRecorderFactory TypeParameter { get; }
    private IMappedSemanticConstructorArgumentRecorderFactory ConstructorParameter { get; }
    private IMappedSemanticNamedArgumentRecorderFactory NamedParameter { get; }

    /// <summary>Instantiates a <see cref="MappedSemanticArgumentRecorderFactory"/>, handling creation of attached recorders.</summary>
    /// <param name="typeParameter">Handles creation of attached recorders related to type parameters.</param>
    /// <param name="constructorParameter">Handles creation of attached recorders related to constructor parameters.</param>
    /// <param name="namedParameter">Handles creation of attached recorders related to named parameters.</param>
    public MappedSemanticArgumentRecorderFactory(IMappedSemanticTypeArgumentRecorderFactory typeParameter, IMappedSemanticConstructorArgumentRecorderFactory constructorParameter, IMappedSemanticNamedArgumentRecorderFactory namedParameter)
    {
        TypeParameter = typeParameter ?? throw new ArgumentNullException(nameof(typeParameter));
        ConstructorParameter = constructorParameter ?? throw new ArgumentNullException(nameof(constructorParameter));
        NamedParameter = namedParameter ?? throw new ArgumentNullException(nameof(namedParameter));
    }

    IMappedSemanticTypeArgumentRecorderFactory IMappedSemanticArgumentRecorderFactory.TypeParameter => TypeParameter;
    IMappedSemanticConstructorArgumentRecorderFactory IMappedSemanticArgumentRecorderFactory.ConstructorParameter => ConstructorParameter;
    IMappedSemanticNamedArgumentRecorderFactory IMappedSemanticArgumentRecorderFactory.NamedParameter => NamedParameter;
}
