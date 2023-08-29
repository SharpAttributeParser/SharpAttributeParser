namespace SharpAttributeParser.Logging.SyntacticParserComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="ISyntacticParser"/> when related to type arguments.</summary>
public interface ITypeArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to parse the type arguments of an attribute.</summary>
    /// <param name="parameters">The type parameters.</param>
    /// <param name="nameSyntax">The syntactic description of the attribute name.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeParsingTypeArguments(IReadOnlyList<ITypeParameterSymbol> parameters, NameSyntax nameSyntax);

    /// <summary>Begins a log scope describing an attempt to the argument of a type parameter.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeParsingTypeArgument(ITypeParameterSymbol parameter, TypeSyntax syntax);

    /// <summary>Begins a log scope describing the <see cref="NameSyntax"/> being recognized as a <see cref="GenericNameSyntax"/>.</summary>
    /// <param name="genericNameSyntax">The syntactic description of the generic attribute name.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeRecognizedGenericExpression(GenericNameSyntax genericNameSyntax);

    /// <summary>Logs a message describing a failed attempt to recognize the syntax as expressing a generic type.</summary>
    public abstract void SyntaxNotRecognizedAsGenericExpression();

    /// <summary>Logs a message describing an unexpected number of syntactic type arguments.</summary>
    public abstract void UnexpectedNumberOfSyntacticTypeArguments();
}
