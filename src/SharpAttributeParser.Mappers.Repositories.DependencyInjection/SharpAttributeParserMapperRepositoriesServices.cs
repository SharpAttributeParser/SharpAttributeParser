namespace SharpAttributeParser.Mappers.Repositories;

using Microsoft.Extensions.DependencyInjection;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;
using SharpAttributeParser.Patterns;

using System;

/// <summary>Allows the services of <i>SharpAttributeParser.Mappers.Repositories</i> to be registered with a <see cref="IServiceCollection"/>.</summary>
public static class SharpAttributeParserMapperRepositoriesServices
{
    /// <summary>Registers the services of <i>SharpAttributeParser.Mappers.Repositories</i> with the provided <see cref="IServiceCollection"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> with which services are registered.</param>
    /// <returns>The provided <see cref="IServiceCollection"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static IServiceCollection AddSharpAttributeParserMapperRepositories(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSharpAttributeParserPatterns();

        services.AddSingleton(typeof(ITypeMappingRepositoryFactory<,>), typeof(TypeMappingRepositoryFactory<,>));
        services.AddSingleton(typeof(IConstructorMappingRepositoryFactory<,>), typeof(ConstructorMappingRepositoryFactory<,>));
        services.AddSingleton(typeof(INamedMappingRepositoryFactory<,>), typeof(NamedMappingRepositoryFactory<,>));

        services.AddSingleton(typeof(ICombinedMappingRepositoryFactory<>), typeof(CombinedMappingRepositoryFactory<>));
        services.AddSingleton(typeof(ISemanticMappingRepositoryFactory<>), typeof(SemanticMappingRepositoryFactory<>));
        services.AddSingleton(typeof(ISyntacticMappingRepositoryFactory<>), typeof(SyntacticMappingRepositoryFactory<>));
        services.AddSingleton(typeof(IAdaptiveMappingRepositoryFactory<,>), typeof(AdaptiveMappingRepositoryFactory<,>));
        services.AddSingleton(typeof(ISplitMappingRepositoryFactory<,>), typeof(SplitMappingRepositoryFactory<,>));

        services.AddSingleton(typeof(IDetachedMappedCombinedTypeArgumentRecorderFactory<>), typeof(DetachedMappedCombinedTypeArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedCombinedConstructorArgumentRecorderFactory<>), typeof(DetachedMappedCombinedConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<>), typeof(DetachedMappedCombinedNormalConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<>), typeof(DetachedMappedCombinedParamsConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<>), typeof(DetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedCombinedNamedArgumentRecorderFactory<>), typeof(DetachedMappedCombinedNamedArgumentRecorderFactory<>));

        services.AddSingleton<IMappedCombinedArgumentRecorderFactory, MappedCombinedArgumentRecorderFactory>();
        services.AddSingleton<IMappedCombinedTypeArgumentRecorderFactory, MappedCombinedTypeArgumentRecorderFactory>();
        services.AddSingleton<IMappedCombinedConstructorArgumentRecorderFactory, MappedCombinedConstructorArgumentRecorderFactory>();
        services.AddSingleton<IMappedCombinedNamedArgumentRecorderFactory, MappedCombinedNamedArgumentRecorderFactory>();

        services.AddSingleton(typeof(IDetachedMappedSemanticTypeArgumentRecorderFactory<>), typeof(DetachedMappedSemanticTypeArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedSemanticConstructorArgumentRecorderFactory<>), typeof(DetachedMappedSemanticConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedSemanticNamedArgumentRecorderFactory<>), typeof(DetachedMappedSemanticNamedArgumentRecorderFactory<>));

        services.AddSingleton<IMappedSemanticArgumentRecorderFactory, MappedSemanticArgumentRecorderFactory>();
        services.AddSingleton<IMappedSemanticTypeArgumentRecorderFactory, MappedSemanticTypeArgumentRecorderFactory>();
        services.AddSingleton<IMappedSemanticConstructorArgumentRecorderFactory, MappedSemanticConstructorArgumentRecorderFactory>();
        services.AddSingleton<IMappedSemanticNamedArgumentRecorderFactory, MappedSemanticNamedArgumentRecorderFactory>();

        services.AddSingleton(typeof(IDetachedMappedSyntacticTypeArgumentRecorderFactory<>), typeof(DetachedMappedSyntacticTypeArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedSyntacticConstructorArgumentRecorderFactory<>), typeof(DetachedMappedSyntacticConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<>), typeof(DetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<>), typeof(DetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<>), typeof(DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<>));
        services.AddSingleton(typeof(IDetachedMappedSyntacticNamedArgumentRecorderFactory<>), typeof(DetachedMappedSyntacticNamedArgumentRecorderFactory<>));

        services.AddSingleton<IMappedSyntacticArgumentRecorderFactory, MappedSyntacticArgumentRecorderFactory>();
        services.AddSingleton<IMappedSyntacticTypeArgumentRecorderFactory, MappedSyntacticTypeArgumentRecorderFactory>();
        services.AddSingleton<IMappedSyntacticConstructorArgumentRecorderFactory, MappedSyntacticConstructorArgumentRecorderFactory>();
        services.AddSingleton<IMappedSyntacticNamedArgumentRecorderFactory, MappedSyntacticNamedArgumentRecorderFactory>();

        services.AddSingleton(typeof(IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<,>), typeof(DetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<,>), typeof(DetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<,>), typeof(DetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<,>), typeof(DetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<,>), typeof(DetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<,>), typeof(DetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<,>));

        services.AddSingleton(typeof(IDetachedMappedSplitTypeArgumentRecorderProviderFactory<,>), typeof(DetachedMappedSplitTypeArgumentRecorderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<,>), typeof(DetachedMappedSplitConstructorArgumentRecorderProviderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<,>), typeof(DetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<,>), typeof(DetachedMappedSplitParamsConstructorArgumentRecorderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<,>), typeof(DetachedMappedSplitOptionalConstructorArgumentRecorderFactory<,>));
        services.AddSingleton(typeof(IDetachedMappedSplitNamedArgumentRecorderProviderFactory<,>), typeof(DetachedMappedSplitNamedArgumentRecorderProviderFactory<,>));

        return services;
    }
}
