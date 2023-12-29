namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

internal static class DefaultRecorderFactories
{
    private static IMappedSemanticArgumentRecorderFactory? CachedSemanticFactory;
    private static IMappedSyntacticArgumentRecorderFactory? CachedSyntacticFactory;
    private static IMappedCombinedArgumentRecorderFactory? CachedCombinedFactory;

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
