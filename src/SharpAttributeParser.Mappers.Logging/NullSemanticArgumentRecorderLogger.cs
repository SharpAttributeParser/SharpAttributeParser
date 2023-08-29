namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.Logging.SemanticArgumentRecorderComponents;

using System;

/// <summary>A <see cref="ISemanticArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullSemanticArgumentRecorderLogger<TCategoryName> : ISemanticArgumentRecorderLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullSemanticArgumentRecorderLogger{TCategoryName}"/>.</summary>
    public static NullSemanticArgumentRecorderLogger<TCategoryName> Singleton { get; } = new();

    ITypeArgumentLogger ISemanticArgumentRecorderLogger.TypeArgument { get; } = new NullTypeArgumentLogger();
    IConstructorArgumentLogger ISemanticArgumentRecorderLogger.ConstructorArgument { get; } = new NullConstructorArgumentLogger();
    INamedArgumentLogger ISemanticArgumentRecorderLogger.NamedArgument { get; } = new NullNamedArgumentLogger();

    private NullSemanticArgumentRecorderLogger() { }

    private sealed class NullTypeArgumentLogger : ITypeArgumentLogger
    {
        IDisposable? ITypeArgumentLogger.BeginScopeRecordingTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument) => null;

        void ITypeArgumentLogger.FailedToMapTypeParameterToRecorder() { }
    }

    private sealed class NullConstructorArgumentLogger : IConstructorArgumentLogger
    {
        IDisposable? IConstructorArgumentLogger.BeginScopeRecordingConstructorArgument(IParameterSymbol parameter, object? argument) => null;

        void IConstructorArgumentLogger.FailedToMapConstructorParameterToRecorder() { }
    }

    private sealed class NullNamedArgumentLogger : INamedArgumentLogger
    {
        IDisposable? INamedArgumentLogger.BeginScopeRecordingNamedArgument(string parameterName, object? argument) => null;

        void INamedArgumentLogger.FailedToMapNamedParameterToRecorder() { }
    }
}
