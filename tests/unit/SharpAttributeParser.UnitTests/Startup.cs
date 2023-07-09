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

        services.AddSingleton<ISemanticAttributeMapper<ISemanticExampleAttributeRecordBuilder>, SemanticExampleAttributeMapper>();
        services.AddSingleton<ISemanticExampleAttributeRecorderFactory, SemanticExampleAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticGenericAttributeRecordBuilder>, SemanticGenericAttributeMapper>();
        services.AddSingleton<ISemanticGenericAttributeRecorderFactory, SemanticGenericAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticQualifiedGenericAttributeRecordBuilder>, SemanticQualifiedGenericAttributeMapper>();
        services.AddSingleton<ISemanticQualifiedGenericAttributeRecorderFactory, SemanticQualifiedGenericAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticSimpleConstructorAttributeRecordBuilder>, SemanticSimpleConstructorAttributeMapper>();
        services.AddSingleton<ISemanticSimpleConstructorAttributeRecorderFactory, SemanticSimpleConstructorAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticArrayConstructorAttributeRecordBuilder>, SemanticArrayConstructorAttributeMapper>();
        services.AddSingleton<ISemanticArrayConstructorAttributeRecorderFactory, SemanticArrayConstructorAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticParamsAttributeRecordBuilder>, SemanticParamsAttributeMapper>();
        services.AddSingleton<ISemanticParamsAttributeRecorderFactory, SemanticParamsAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticNamedAttributeRecordBuilder>, SemanticNamedAttributeMapper>();
        services.AddSingleton<ISemanticNamedAttributeRecorderFactory, SemanticNamedAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticCombinedAttributeRecordBuilder>, SemanticCombinedAttributeMapper>();
        services.AddSingleton<ISemanticCombinedAttributeRecorderFactory, SemanticCombinedAttributeRecorderFactory>();
    }
}
