namespace SharpAttributeParser.Logging.CombinedParserComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="ICombinedParser"/> when related to type arguments.</summary>
public interface ITypeArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to propagate a parsed type arguments of an attribute.</summary>
    /// <param name="semanticArguments">The semantically parsed arguments.</param>
    /// <param name="syntacticArguments">The syntactically parsed arguments.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopePropagateParsedTypeArguments(IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> semanticArguments, IReadOnlyDictionary<ITypeParameterSymbol, ExpressionSyntax> syntacticArguments);

    /// <summary>Begins a log scope describing an attempt to propagate a semantically parsed argument of a type parameter.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopePropagateSemanticallyParsedTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument);

    /// <summary>Begins a log scope describing an attempt to propagate a syntactically parsed argument of a type parameter.</summary>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopePropagateSyntacticallyParsedTypeArgument(ExpressionSyntax syntax);

    /// <summary>Logs a message describing different number of semantically and syntactically parsed type arguments.</summary>
    public abstract void DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments();

    /// <summary>Logs a message describing a type argument having been parsed semantically, but not syntactically.</summary>
    public abstract void TypeArgumentOnlyRecordedSemantically();
}
