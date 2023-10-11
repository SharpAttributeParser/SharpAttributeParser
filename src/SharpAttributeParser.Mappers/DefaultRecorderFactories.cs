namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>Provides common functionality related to creating default recorders.</summary>
internal static class DefaultRecorderFactories
{
    private static IMappedSemanticArgumentRecorderFactory? CachedSemanticFactory;
    private static IMappedSyntacticArgumentRecorderFactory? CachedSyntacticFactory;
    private static IMappedCombinedArgumentRecorderFactory? CachedCombinedFactory;

    /// <summary>Creates the default <see cref="IMappedSemanticArgumentRecorderFactory"/>.</summary>
    /// <returns>The default <see cref="IMappedSemanticArgumentRecorderFactory"/>.</returns>
    public static IMappedSemanticArgumentRecorderFactory SemanticFactory()
    {
        if (CachedSemanticFactory is not null)
        {
            return CachedSemanticFactory;
        }

        MappedSemanticTypeArgumentRecorderFactory typeParameter = new();
        MappedSemanticConstructorArgumentRecorderFactory constructorParameter = new();
        MappedSemanticNamedArgumentRecorderFactory namedParameter = new();

        return CachedSemanticFactory = new MappedSemanticArgumentRecorderFactory(typeParameter, constructorParameter, namedParameter);
    }

    /// <summary>Creates the default <see cref="IMappedSyntacticArgumentRecorderFactory"/>.</summary>
    /// <returns>The default <see cref="IMappedSyntacticArgumentRecorderFactory"/>.</returns>
    public static IMappedSyntacticArgumentRecorderFactory SyntacticFactory()
    {
        if (CachedSyntacticFactory is not null)
        {
            return CachedSyntacticFactory;
        }

        MappedSyntacticTypeArgumentRecorderFactory typeParameter = new();
        MappedSyntacticConstructorArgumentRecorderFactory constructorParameter = new();
        MappedSyntacticNamedArgumentRecorderFactory namedParameter = new();

        return CachedSyntacticFactory = new MappedSyntacticArgumentRecorderFactory(typeParameter, constructorParameter, namedParameter);
    }

    /// <summary>Creates the default <see cref="IMappedCombinedArgumentRecorderFactory"/>.</summary>
    /// <returns>The default <see cref="IMappedCombinedArgumentRecorderFactory"/>.</returns>
    public static IMappedCombinedArgumentRecorderFactory CombinedFactory()
    {
        if (CachedCombinedFactory is not null)
        {
            return CachedCombinedFactory;
        }

        MappedCombinedTypeArgumentRecorderFactory typeParameter = new();
        MappedCombinedConstructorArgumentRecorderFactory constructorParameter = new();
        MappedCombinedNamedArgumentRecorderFactory namedParameter = new();

        return CachedCombinedFactory = new MappedCombinedArgumentRecorderFactory(typeParameter, constructorParameter, namedParameter);
    }
}
