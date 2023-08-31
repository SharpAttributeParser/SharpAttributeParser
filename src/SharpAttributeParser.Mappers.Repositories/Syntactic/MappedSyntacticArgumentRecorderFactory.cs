namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;

/// <inheritdoc cref="IMappedSyntacticArgumentRecorderFactory"/>
public sealed class MappedSyntacticArgumentRecorderFactory : IMappedSyntacticArgumentRecorderFactory
{
    private IMappedSyntacticTypeArgumentRecorderFactory TypeParameter { get; }
    private IMappedSyntacticConstructorArgumentRecorderFactory ConstructorParameter { get; }
    private IMappedSyntacticNamedArgumentRecorderFactory NamedParameter { get; }

    /// <summary>Instantiates a <see cref="MappedSyntacticArgumentRecorderFactory"/>, handling creation of attached recorders.</summary>
    /// <param name="typeParameter">Handles creation of attached recorders related to type parameters.</param>
    /// <param name="constructorParameter">Handles creation of attached recorders related to constructor parameters.</param>
    /// <param name="namedParameter">Handles creation of attached recorders related to named parameters.</param>
    public MappedSyntacticArgumentRecorderFactory(IMappedSyntacticTypeArgumentRecorderFactory typeParameter, IMappedSyntacticConstructorArgumentRecorderFactory constructorParameter, IMappedSyntacticNamedArgumentRecorderFactory namedParameter)
    {
        TypeParameter = typeParameter ?? throw new ArgumentNullException(nameof(typeParameter));
        ConstructorParameter = constructorParameter ?? throw new ArgumentNullException(nameof(constructorParameter));
        NamedParameter = namedParameter ?? throw new ArgumentNullException(nameof(namedParameter));
    }

    IMappedSyntacticTypeArgumentRecorderFactory IMappedSyntacticArgumentRecorderFactory.TypeParameter => TypeParameter;
    IMappedSyntacticConstructorArgumentRecorderFactory IMappedSyntacticArgumentRecorderFactory.ConstructorParameter => ConstructorParameter;
    IMappedSyntacticNamedArgumentRecorderFactory IMappedSyntacticArgumentRecorderFactory.NamedParameter => NamedParameter;
}
