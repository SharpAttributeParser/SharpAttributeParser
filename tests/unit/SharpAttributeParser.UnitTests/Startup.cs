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
        services.AddSingleton<ISyntacticAttributeMapper<ISyntacticGenericAttributeRecordBuilder>, SyntacticGenericAttributeMapper>();
        services.AddSingleton<ISemanticGenericAttributeRecorderFactory, SemanticGenericAttributeRecorderFactory>();
        services.AddSingleton<ISyntacticGenericAttributeRecorderFactory, SyntacticGenericAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticQualifiedGenericAttributeRecordBuilder>, SemanticQualifiedGenericAttributeMapper>();
        services.AddSingleton<ISyntacticAttributeMapper<ISyntacticQualifiedGenericAttributeRecordBuilder>, SyntacticQualifiedGenericAttributeMapper>();
        services.AddSingleton<ISemanticQualifiedGenericAttributeRecorderFactory, SemanticQualifiedGenericAttributeRecorderFactory>();
        services.AddSingleton<ISyntacticQualifiedGenericAttributeRecorderFactory, SyntacticQualifiedGenericAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticSimpleConstructorAttributeRecordBuilder>, SemanticSimpleConstructorAttributeMapper>();
        services.AddSingleton<ISyntacticAttributeMapper<ISyntacticSimpleConstructorAttributeRecordBuilder>, SyntacticSimpleConstructorAttributeMapper>();
        services.AddSingleton<ISemanticSimpleConstructorAttributeRecorderFactory, SemanticSimpleConstructorAttributeRecorderFactory>();
        services.AddSingleton<ISyntacticSimpleConstructorAttributeRecorderFactory, SyntacticSimpleConstructorAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticArrayConstructorAttributeRecordBuilder>, SemanticArrayConstructorAttributeMapper>();
        services.AddSingleton<ISyntacticAttributeMapper<ISyntacticArrayConstructorAttributeRecordBuilder>, SyntacticArrayConstructorAttributeMapper>();
        services.AddSingleton<ISemanticArrayConstructorAttributeRecorderFactory, SemanticArrayConstructorAttributeRecorderFactory>();
        services.AddSingleton<ISyntacticArrayConstructorAttributeRecorderFactory, SyntacticArrayConstructorAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticParamsAttributeRecordBuilder>, SemanticParamsAttributeMapper>();
        services.AddSingleton<ISyntacticAttributeMapper<ISyntacticParamsAttributeRecordBuilder>, SyntacticParamsAttributeMapper>();
        services.AddSingleton<ISemanticParamsAttributeRecorderFactory, SemanticParamsAttributeRecorderFactory>();
        services.AddSingleton<ISyntacticParamsAttributeRecorderFactory, SyntacticParamsAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticNamedAttributeRecordBuilder>, SemanticNamedAttributeMapper>();
        services.AddSingleton<ISyntacticAttributeMapper<ISyntacticNamedAttributeRecordBuilder>, SyntacticNamedAttributeMapper>();
        services.AddSingleton<ISemanticNamedAttributeRecorderFactory, SemanticNamedAttributeRecorderFactory>();
        services.AddSingleton<ISyntacticNamedAttributeRecorderFactory, SyntacticNamedAttributeRecorderFactory>();

        services.AddSingleton<ISemanticAttributeMapper<ISemanticCombinedAttributeRecordBuilder>, SemanticCombinedAttributeMapper>();
        services.AddSingleton<ISyntacticAttributeMapper<ISyntacticCombinedAttributeRecordBuilder>, SyntacticCombinedAttributeMapper>();
        services.AddSingleton<ISemanticCombinedAttributeRecorderFactory, SemanticCombinedAttributeRecorderFactory>();
        services.AddSingleton<ISyntacticCombinedAttributeRecorderFactory, SyntacticCombinedAttributeRecorderFactory>();
    }
}
