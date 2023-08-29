namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging.SyntacticMapperComponents;
using SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>A <see cref="ISyntacticMapperLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullSyntacticMapperLogger<TCategoryName> : ISyntacticMapperLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullSyntacticMapperLogger{TCategoryName}"/>.</summary>
    public static NullSyntacticMapperLogger<TCategoryName> Singleton { get; } = new();

    ITypeParameterLogger ISyntacticMapperLogger.TypeParameter { get; } = new NullTypeParameterLogger();
    IConstructorParameterLogger ISyntacticMapperLogger.ConstructorParameter { get; } = new NullConstructorParameterLogger();
    INamedParameterLogger ISyntacticMapperLogger.NamedParameter { get; } = new NullNamedParameterLogger();

    private NullSyntacticMapperLogger() { }

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
