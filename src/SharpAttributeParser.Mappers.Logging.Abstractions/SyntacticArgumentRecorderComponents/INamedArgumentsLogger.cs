namespace SharpAttributeParser.Mappers.Logging.SyntacticArgumentRecorderComponents;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Handles logging for <see cref="ISyntacticRecorder"/> when related to named arguments.</summary>
public interface INamedArgumentsLogger
{
    /// <summary>Begins a log scope describing an attempt to record syntactic information about an argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="syntax">The syntactic description of the named argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeRecordingNamedArgument(string parameterName, ExpressionSyntax syntax);

    /// <summary>Logs a message describing a failed attempt to map a named parameter to a recorder.</summary>
    public abstract void FailedToMapNamedParameterToRecorder();
}
