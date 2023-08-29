namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Logging.CombinedParserComponents;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>Handles logging for <see cref="ICombinedParser"/>.</summary>
public interface ICombinedParserLogger
{
    /// <summary>Handles logging related to type arguments.</summary>
    public abstract ITypeArgumentLogger TypeArgument { get; }

    /// <summary>Handles logging related to constructor arguments.</summary>
    public abstract IConstructorArgumentLogger ConstructorArgument { get; }

    /// <summary>Handles logging related to named arguments.</summary>
    public abstract INamedArgumentLogger NamedArgument { get; }

    /// <summary>Begins a log scope describing an attempt to parse an attribute.</summary>
    /// <param name="recorderType">The type of the recorder used to record the parsed arguments.</param>
    /// <param name="attributeData">The semantic description of the attribute.</param>
    /// <param name="attributeSyntax">The syntactic description of the attribute.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData, AttributeSyntax attributeSyntax);

    /// <summary>Logs a message describing a <see cref="ISemanticParser"/> failing to parse an attribute.</summary>
    public abstract void SemanticParserFailedToParseAttribute();

    /// <summary>Logs a message describing a <see cref="ISyntacticParser"/> failing to parse an attribute.</summary>
    public abstract void SyntacticParserFailedToParseAttribute();
}

/// <inheritdoc cref="ICombinedParserLogger"/>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Follows the pattern of ILogger<CategoryName>")]
public interface ICombinedParserLogger<out TCategoryName> : ICombinedParserLogger { }
