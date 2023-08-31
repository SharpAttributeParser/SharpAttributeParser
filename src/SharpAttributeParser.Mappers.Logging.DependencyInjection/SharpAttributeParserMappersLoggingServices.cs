namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.Extensions.DependencyInjection;

using SharpAttributeParser.Logging;
using SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>Allows the services of <i>SharpAttributeParser.Mappers.Logging</i> to be registered with a <see cref="IServiceCollection"/>.</summary>
public static class SharpAttributeParserMappersLoggingServices
{
    /// <summary>Registers the services of <i>SharpAttributeParser.Mappers.Logging</i> with the provided <see cref="IServiceCollection"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> with which services are registered.</param>
    /// <returns>The provided <see cref="IServiceCollection"/>, so that calls can be chained.</returns>
    public static IServiceCollection AddSharpAttributeParserMappersLogging(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSharpAttributeParserLogging();
        services.AddSharpAttributeParserMapperRepositoriesLogging();

        services.AddSingleton(typeof(ICombinedArgumentRecorderLogger<>), typeof(CombinedArgumentRecorderLogger<>));
        services.AddSingleton(typeof(ISemanticArgumentRecorderLogger<>), typeof(SemanticArgumentRecorderLogger<>));
        services.AddSingleton(typeof(ISyntacticArgumentRecorderLogger<>), typeof(SyntacticArgumentRecorderLogger<>));

        services.AddSingleton<ICombinedArgumentRecorderLoggerFactory, CombinedArgumentRecorderLoggerFactory>();
        services.AddSingleton<ISemanticArgumentRecorderLoggerFactory, SemanticArgumentRecorderLoggerFactory>();
        services.AddSingleton<ISyntacticArgumentRecorderLoggerFactory, SyntacticArgumentRecorderLoggerFactory>();

        services.AddSingleton(typeof(ISemanticMapperLogger<>), typeof(SemanticMapperLogger<>));
        services.AddSingleton(typeof(ISyntacticMapperLogger<>), typeof(SyntacticMapperLogger<>));
        services.AddSingleton(typeof(ICombinedMapperLogger<>), typeof(CombinedMapperLogger<>));

        return services;
    }
}
