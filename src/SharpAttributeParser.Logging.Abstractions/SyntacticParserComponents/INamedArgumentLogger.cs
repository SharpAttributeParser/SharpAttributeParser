namespace SharpAttributeParser.Logging.SyntacticParserComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Handles logging for <see cref="ISyntacticParser"/> when related to named arguments.</summary>
public interface INamedArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to parse the named arguments of an attribute.</summary>
    /// <param name="attributeData">The semantic description of the attribute.</param>
    /// <param name="attributeSyntax">The syntactic description of the attribute.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingNamedArguments(AttributeData attributeData, AttributeSyntax attributeSyntax);

    /// <summary>Begins a log scope describing an attempt to parse the argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingNamedArgument(string parameterName, ExpressionSyntax syntax);
}
