namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging.CombinedMapperComponents;
using SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>A <see cref="ICombinedMapperLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullCombinedMapperLogger<TCategoryName> : ICombinedMapperLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullCombinedMapperLogger{TCategoryName}"/>.</summary>
    public static NullCombinedMapperLogger<TCategoryName> Singleton { get; } = new();

    ITypeParameterLogger ICombinedMapperLogger.TypeParameter { get; } = new NullTypeParameterLogger();
    IConstructorParameterLogger ICombinedMapperLogger.ConstructorParameter { get; } = new NullConstructorParameterLogger();
    INamedParameterLogger ICombinedMapperLogger.NamedParameter { get; } = new NullNamedParameterLogger();

    private NullCombinedMapperLogger() { }

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
