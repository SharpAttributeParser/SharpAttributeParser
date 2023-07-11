namespace SharpAttributeParser.Extensions;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Hosts extensions methods for <see cref="IServiceCollection"/></summary>
public static class IServiceCollectionExtensions
{
    /// <summary>Adds services offered by <i>SharpAttributeParser</i> to the provided <see cref="IServiceCollection"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services are added.</param>
    /// <returns>The provided <see cref="IServiceCollection"/>, so that calls can be chained.</returns>
    public static IServiceCollection AddSharpAttributeParser(this IServiceCollection services)
    {
        services.AddSingleton<ISemanticAttributeParser, SemanticAttributeParser>();
        services.AddSingleton<ISyntacticAttributeParser, SyntacticAttributeParser>();

        services.AddSingleton<ISemanticAttributeRecorderFactory, SemanticAttributeRecorderFactory>();
        services.AddSingleton<ISyntacticAttributeRecorderFactory, SyntacticAttributeRecorderFactory>();

        return services;
    }
}
