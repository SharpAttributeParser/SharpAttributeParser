namespace SharpAttributeParser.Logging;

using Microsoft.Extensions.DependencyInjection;

using System;

/// <summary>Allows the services of <i>SharpAttributeParser.Logging</i> to be registered with a <see cref="IServiceCollection"/>.</summary>
public static class SharpAttributeParserLoggingServices
{
    /// <summary>Registers the services of <i>SharpAttributeParser.Logging</i> with the provided <see cref="IServiceCollection"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> with which services are registered.</param>
    /// <returns>The provided <see cref="IServiceCollection"/>, so that calls can be chained.</returns>
    public static IServiceCollection AddSharpAttributeParserLogging(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddLogging();

        services.AddSingleton(typeof(ICombinedParserLogger<>), typeof(CombinedParserLogger<>));
        services.AddSingleton(typeof(ISyntacticParserLogger<>), typeof(SyntacticParserLogger<>));
        services.AddSingleton(typeof(ISemanticParserLogger<>), typeof(SemanticParserLogger<>));

        return services;
    }
}
