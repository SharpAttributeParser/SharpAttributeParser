namespace SharpAttributeParser.Logging.SemanticParserComponents;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="ISemanticParser"/> when related to named arguments.</summary>
public interface INamedArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to parse the named arguments of an attribute.</summary>
    /// <param name="arguments">The named parameters and the associated argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeParsingNamedArguments(IReadOnlyList<KeyValuePair<string, TypedConstant>> arguments);

    /// <summary>Begins a log scope describing an attempt to parse an argument of a named argument.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="argument">The argument of the named parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeParsingNamedArgument(string parameterName, TypedConstant argument);

    /// <summary>Logs a message describing a failed attempt to parse a named argument, as the argument was invalid.</summary>
    public abstract void InvalidNamedArgument();
}
