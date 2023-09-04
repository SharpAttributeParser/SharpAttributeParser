namespace SharpAttributeParser;

using Microsoft.Extensions.DependencyInjection;

using SharpAttributeParser.ExampleCases.RecommendedPatternCases;
using SharpAttributeParser.Mappers;

internal static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSharpAttributeParser();
        services.AddSharpAttributeParserMappers();

        services.AddSingleton<ISemanticMapper<IExampleRecordBuilder>, ExampleMapper>();
        services.AddSingleton<IExampleRecorderFactory, ExampleRecorderFactory>();
        services.AddSingleton<IExampleParser, ExampleParser>();
    }
}
