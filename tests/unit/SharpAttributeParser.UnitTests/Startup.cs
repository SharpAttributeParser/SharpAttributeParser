namespace SharpAttributeParser;

using Microsoft.Extensions.DependencyInjection;

using SharpAttributeParser.Extensions;
using SharpAttributeParser.Recording;

using System.Diagnostics.CodeAnalysis;

public sealed class Startup
{
    [SuppressMessage("Performance", "CA1822: Mark members as static", Justification = "<Pending>")]
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSharpAttributeParser();

        services.AddSingleton<ISemanticAttributeMapper<IExampleAttributeDataBuilder>, SemanticExampleAttributeMapper>();
        services.AddSingleton<ISemanticExampleAttributeRecorderFactory, SemanticExampleAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<IGenericAttributeDataBuilder>, SemanticGenericAttributeMapper>();
        services.AddSingleton<ISemanticGenericAttributeRecorderFactory, SemanticGenericAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<IQualifiedGenericAttributeDataBuilder>, SemanticQualifiedGenericAttributeMapper>();
        services.AddSingleton<ISemanticQualifiedGenericAttributeRecorderFactory, SemanticQualifiedGenericAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISimpleConstructorAttributeDataBuilder>, SemanticSimpleConstructorAttributeMapper>();
        services.AddSingleton<ISemanticSimpleConstructorAttributeRecorderFactory, SemanticSimpleConstructorAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<IArrayConstructorAttributeDataBuilder>, SemanticArrayConstructorAttributeMapper>();
        services.AddSingleton<ISemanticArrayConstructorAttributeRecorderFactory, SemanticArrayConstructorAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<IParamsAttributeDataBuilder>, SemanticParamsAttributeMapper>();
        services.AddSingleton<ISemanticParamsAttributeRecorderFactory, SemanticParamsAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<INamedAttributeDataBuilder>, SemanticNamedAttributeMapper>();
        services.AddSingleton<ISemanticNamedAttributeRecorderFactory, SemanticNamedAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ICombinedAttributeDataBuilder>, SemanticCombinedAttributeMapper>();
        services.AddSingleton<ISemanticCombinedAttributeRecorderFactory, SemanticCombinedAttributeRecorderFactory>();
    }
}
