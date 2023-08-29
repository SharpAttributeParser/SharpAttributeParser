namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.Logging.SyntacticArgumentRecorderComponents;

using System;
using System.Collections.Generic;

/// <summary>A <see cref="ISyntacticArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
public sealed class NullSyntacticArgumentRecorderLogger<TCategoryName> : ISyntacticArgumentRecorderLogger<TCategoryName>
{
    /// <summary>The singleton <see cref="NullSyntacticArgumentRecorderLogger{TCategoryName}"/>.</summary>
    public static NullSyntacticArgumentRecorderLogger<TCategoryName> Singleton { get; } = new();

    ITypeArgumentsLogger ISyntacticArgumentRecorderLogger.TypeArgument { get; } = new NullTypeArgumentLogger();
    IConstructorArgumentsLogger ISyntacticArgumentRecorderLogger.ConstructorArgument { get; } = new NullConstructorArgumentLogger();
    INamedArgumentsLogger ISyntacticArgumentRecorderLogger.NamedArgument { get; } = new NullNamedArgumentLogger();

    private NullSyntacticArgumentRecorderLogger() { }

    private sealed class NullTypeArgumentLogger : ITypeArgumentsLogger
    {
        IDisposable? ITypeArgumentsLogger.BeginScopeRecordingTypeArgument(ITypeParameterSymbol parameter, ExpressionSyntax syntax) => null;

        void ITypeArgumentsLogger.FailedToMapTypeParameterToRecorder() { }
    }

    private sealed class NullConstructorArgumentLogger : IConstructorArgumentsLogger
    {
        IDisposable? IConstructorArgumentsLogger.BeginScopeRecordingNormalConstructorArgument(IParameterSymbol parameter, ExpressionSyntax syntax) => null;
        IDisposable? IConstructorArgumentsLogger.BeginScopeRecordingParamsConstructorArgument(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax) => null;
        IDisposable? IConstructorArgumentsLogger.BeginScopeRecordingDefaultConstructorArgument(IParameterSymbol parameter) => null;

        void IConstructorArgumentsLogger.FailedToMapConstructorParameterToRecorder() { }
    }

    private sealed class NullNamedArgumentLogger : INamedArgumentsLogger
    {
        IDisposable? INamedArgumentsLogger.BeginScopeRecordingNamedArgument(string parameterName, ExpressionSyntax syntax) => null;

        void INamedArgumentsLogger.FailedToMapNamedParameterToRecorder() { }
    }
}
