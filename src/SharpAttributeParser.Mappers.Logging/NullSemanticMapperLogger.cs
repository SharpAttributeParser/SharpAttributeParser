namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging.SemanticMapperComponents;
using SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>A <see cref="ISemanticMapperLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullSemanticMapperLogger<TCategoryName> : ISemanticMapperLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullSemanticMapperLogger{TCategoryName}"/>.</summary>
    public static NullSemanticMapperLogger<TCategoryName> Singleton { get; } = new();

    ITypeParameterLogger ISemanticMapperLogger.TypeParameter { get; } = new NullTypeParameterLogger();
    IConstructorParameterLogger ISemanticMapperLogger.ConstructorParameter { get; } = new NullConstructorParameterLogger();
    INamedParameterLogger ISemanticMapperLogger.NamedParameter { get; } = new NullNamedParameterLogger();

    private NullSemanticMapperLogger() { }

    private sealed class NullTypeParameterLogger : ITypeParameterLogger
    {
        IDisposable? ITypeParameterLogger.BeginScopeMappingTypeParameter<TRecorder>(ITypeParameterSymbol parameter, IBuiltTypeMappingRepository<TRecorder> mappingRepository) => null;

        void ITypeParameterLogger.FailedToMapTypeParameter() { }
    }

    private sealed class NullConstructorParameterLogger : IConstructorParameterLogger
    {
        IDisposable? IConstructorParameterLogger.BeginScopeMappingConstructorParameter<TRecorder>(IParameterSymbol parameter, IBuiltConstructorMappingRepository<TRecorder> mappingRepository) => null;

        void IConstructorParameterLogger.FailedToMapConstructorParameter() { }
    }

    private sealed class NullNamedParameterLogger : INamedParameterLogger
    {
        IDisposable? INamedParameterLogger.BeginScopeMappingNamedParameter<TRecorder>(string parameterName, IBuiltNamedMappingRepository<TRecorder> mappingRepository) => null;

        void INamedParameterLogger.FailedToMapNamedParameter() { }
    }
}
