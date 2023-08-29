namespace SharpAttributeParser.Mappers;

using Microsoft.Extensions.DependencyInjection;

using SharpAttributeParser.Mappers.Logging;
using SharpAttributeParser.Mappers.Repositories;
using SharpAttributeParser.Patterns;

using System;

/// <summary>Allows the services of <i>SharpAttributeParser.Mappers</i> to be registered with a <see cref="IServiceCollection"/>.</summary>
public static class SharpAttributeParserMappersServices
{
    /// <summary>Registers the services of <i>SharpAttributeParser.Mappers</i> with the provided <see cref="IServiceCollection"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> with which services are registered.</param>
    /// <returns>The provided <see cref="IServiceCollection"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static IServiceCollection AddSharpAttributeParserMappers(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSharpAttributeParser();
        services.AddSharpAttributeParserPatterns();
        services.AddSharpAttributeParserMapperRepositories();

        services.AddSharpAttributeParserMappersLogging();

        services.AddSingleton<ICombinedRecorderFactory, CombinedRecorderFactory>();
        services.AddSingleton<ISemanticRecorderFactory, SemanticRecorderFactory>();
        services.AddSingleton<ISyntacticRecorderFactory, SyntacticRecorderFactory>();

        return services;
    }
}
