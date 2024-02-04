namespace SharpAttributeParser.Logging.ParserLoggerComponents;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="IParser"/> when related to named arguments.</summary>
public interface INamedArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to propagate the parsed named arguments of an attribute.</summary>
    /// <param name="semanticArguments">The semantically parsed arguments.</param>
    /// <param name="syntacticArguments">The syntactically parsed arguments.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopePropagateParsedNamedArguments(IReadOnlyDictionary<string, object?> semanticArguments, IReadOnlyDictionary<string, ExpressionSyntax> syntacticArguments);

    /// <summary>Begins a log scope describing an attempt to propagate a semantically parsed argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="argument">The argument of the named parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopePropagateSemanticallyParsedNamedArgument(string parameterName, object? argument);

    /// <summary>Begins a log scope describing an attempt to propagate a syntactically parsed argument of a named parameter.</summary>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopePropagateSyntacticallyParsedNamedArgument(ExpressionSyntax syntax);

    /// <summary>Logs a message describing different number of semantically and syntactically parsed named arguments.</summary>
    public abstract void DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments();

    /// <summary>Logs a message describing a named argument having been parsed semantically, but not syntactically.</summary>
    public abstract void NamedArgumentOnlyRecordedSemantically();
}
