namespace SharpAttributeParser.Mappers.Repositories;

using Microsoft.Extensions.DependencyInjection;

using SharpAttributeParser.Mappers.Repositories.Logging.Combined;
using SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using System;

/// <summary>Allows the services of <i>SharpAttributeParser.Mappers.Repositories.Logging</i> to be registered with a <see cref="IServiceCollection"/>.</summary>
public static class SharpAttributeParserMapperRepositoriesServices
{
    /// <summary>Registers the services of <i>SharpAttributeParser.Mappers.Repositories.Logging</i> with the provided <see cref="IServiceCollection"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> with which services are registered.</param>
    /// <returns>The provided <see cref="IServiceCollection"/>, so that calls can be chained.</returns>
    public static IServiceCollection AddSharpAttributeParserMapperRepositoriesLogging(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddLogging();

        services.AddSingleton(typeof(IMappedCombinedConstructorArgumentRecorderLogger<>), typeof(MappedCombinedConstructorArgumentRecorderLogger<>));
        services.AddSingleton(typeof(IMappedCombinedNamedArgumentRecorderLogger<>), typeof(MappedCombinedNamedArgumentRecorderLogger<>));

        services.AddSingleton(typeof(IMappedSemanticConstructorArgumentRecorderLogger<>), typeof(MappedSemanticConstructorArgumentRecorderLogger<>));
        services.AddSingleton(typeof(IMappedSemanticNamedArgumentRecorderLogger<>), typeof(MappedSemanticNamedArgumentRecorderLogger<>));

        services.AddSingleton<IMappedCombinedConstructorArgumentRecorderLoggerFactory, MappedCombinedConstructorArgumentRecorderLoggerFactory>();
        services.AddSingleton<IMappedCombinedNamedArgumentRecorderLoggerFactory, MappedCombinedNamedArgumentRecorderLoggerFactory>();

        services.AddSingleton<IMappedSemanticConstructorArgumentRecorderLoggerFactory, MappedSemanticConstructorArgumentRecorderLoggerFactory>();
        services.AddSingleton<IMappedSemanticNamedArgumentRecorderLoggerFactory, MappedSemanticNamedArgumentRecorderLoggerFactory>();

        return services;
    }
}
