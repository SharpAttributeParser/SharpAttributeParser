namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Logging.SemanticParserComponents;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>Handles logging for <see cref="ISemanticParser"/>.</summary>
public interface ISemanticParserLogger
{
    /// <summary>Handles logging related to type arguments.</summary>
    public abstract ITypeArgumentLogger TypeArgument { get; }

    /// <summary>Handles logging related to constructor arguments.</summary>
    public abstract IConstructorArgumentLogger ConstructorArgument { get; }

    /// <summary>Handles logging related to named arguments.</summary>
    public abstract INamedArgumentLogger NamedArgument { get; }

    /// <summary>Begins a log scope describing an attempt to parse an attribute.</summary>
    /// <param name="recorderType">The type of the recorder used to record the parsed arguments.</param>
    /// <param name="attributeData">The syntactic description of the attribute.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData);

    /// <summary>Begins a log scope describing a determined attribute-class.</summary>
    /// <param name="attributeClass">The attribute-class.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeDeterminedAttributeClass(INamedTypeSymbol attributeClass);

    /// <summary>Begins a log scope describing a determined target constructor.</summary>
    /// <param name="targetConstructor">The target constructor.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeDeterminedTargetConstructor(IMethodSymbol targetConstructor);

    /// <summary>Logs a message describing the attribute-class of the attribute being undetermined.</summary>
    public abstract void UndeterminedAttributeClass();

    /// <summary>Logs a message describing the attribute-class of the attribute being unrecognized.</summary>
    public abstract void UnrecognizedAttributeClass();

    /// <summary>Logs a message describing the targetted constructor of the attribute-class being undetermined.</summary>
    public abstract void UndeterminedTargetConstructor();
}

/// <inheritdoc cref="ISemanticParserLogger"/>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Follows the pattern of ILogger<CategoryName>")]
public interface ISemanticParserLogger<out TCategoryName> : ISemanticParserLogger { }
