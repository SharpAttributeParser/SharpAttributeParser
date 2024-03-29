﻿namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Records the arguments of named parameters, together with syntactic information about the arguments.</summary>
public interface INamedRecorder
{
    /// <summary>Attempts to record an argument of a named parameter, together with syntactic information about the argument.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="argument">The argument of the named parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(string parameterName, object? argument, ExpressionSyntax syntax);
}
