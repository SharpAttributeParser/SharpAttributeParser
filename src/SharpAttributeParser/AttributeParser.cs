namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISemanticAttributeParser"/>
public sealed class AttributeParser : IAttributeParser
{
    private ISemanticAttributeParser SemanticParser { get; }
    private ISyntacticAttributeParser SyntacticParser { get; }

    /// <summary>Instantiates an <see cref="AttributeParser"/>, parsing attribute arguments.</summary>
    /// <param name="semanticParser">The <see cref="ISemanticAttributeParser"/> used to semantically parse attribute arguments.</param>
    /// <param name="syntacticParser">The <see cref="ISyntacticAttributeParser"/> used to syntactically parse attribute arguments.</param>
    /// <exception cref="ArgumentNullException"/>
    public AttributeParser(ISemanticAttributeParser semanticParser, ISyntacticAttributeParser syntacticParser)
    {
        SemanticParser = semanticParser ?? throw new ArgumentNullException(nameof(semanticParser));
        SyntacticParser = syntacticParser ?? throw new ArgumentNullException(nameof(syntacticParser));
    }

    /// <inheritdoc/>
    public bool TryParse(IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        if (attributeData is null)
        {
            throw new ArgumentNullException(nameof(attributeData));
        }

        if (attributeSyntax is null)
        {
            throw new ArgumentNullException(nameof(attributeSyntax));
        }

        SemanticAttributeRecorder semanticRecorder = new();
        SyntacticAttributeRecorder syntacticRecorder = new();

        if (SemanticParser.TryParse(semanticRecorder, attributeData) is false || SyntacticParser.TryParse(syntacticRecorder, attributeData, attributeSyntax) is false)
        {
            return false;
        }

        return PropagateRecordedArguments(recorder, semanticRecorder, syntacticRecorder);
    }

    private bool PropagateRecordedArguments(IAttributeRecorder recorder, SemanticAttributeRecorder semanticRecorder, SyntacticAttributeRecorder syntacticRecorder)
    {
        return PropagateRecordedTypeArguments(recorder, semanticRecorder, syntacticRecorder) && PropagateRecordedConstructorArguments(recorder, semanticRecorder, syntacticRecorder)
            && PropagateRecordedNamedArguments(recorder, semanticRecorder, syntacticRecorder);
    }

    private bool PropagateRecordedTypeArguments(IAttributeRecorder recorder, SemanticAttributeRecorder semanticRecorder, SyntacticAttributeRecorder syntacticRecorder)
    {
        if (semanticRecorder.TypeArguments.Count != syntacticRecorder.TypeArguments.Count)
        {
            return false;
        }

        foreach (var typeArgument in semanticRecorder.TypeArguments)
        {
            if (syntacticRecorder.TypeArguments.TryGetValue(typeArgument.Key, out var syntax) is false)
            {
                return false;
            }

            if (recorder.TryRecordTypeArgument(typeArgument.Key, typeArgument.Value, syntax) is false)
            {
                return false;
            }
        }

        return true;
    }

    private bool PropagateRecordedConstructorArguments(IAttributeRecorder recorder, SemanticAttributeRecorder semanticRecorder, SyntacticAttributeRecorder syntacticRecorder)
    {
        if (semanticRecorder.ConstructorArguments.Count != syntacticRecorder.ConstructorArguments.Count)
        {
            return false;
        }

        foreach (var constructorArgument in semanticRecorder.ConstructorArguments)
        {
            if (syntacticRecorder.ConstructorArguments.TryGetValue(constructorArgument.Key, out var syntax) is false)
            {
                return false;
            }

            var success = syntax.Match
            (
                (syntax) => recorder.TryRecordConstructorArgument(constructorArgument.Key, constructorArgument.Value, syntax),
                (elementSyntax) => recorder.TryRecordConstructorParamsArgument(constructorArgument.Key, constructorArgument.Value, elementSyntax)
            );

            if (success is false)
            {
                return false;
            }
        }

        return true;
    }

    private bool PropagateRecordedNamedArguments(IAttributeRecorder recorder, SemanticAttributeRecorder semanticRecorder, SyntacticAttributeRecorder syntacticRecorder)
    {
        if (semanticRecorder.NamedArguments.Count != syntacticRecorder.NamedArguments.Count)
        {
            return false;
        }

        foreach (var namedArgument in semanticRecorder.NamedArguments)
        {
            if (syntacticRecorder.NamedArguments.TryGetValue(namedArgument.Key, out var syntax) is false)
            {
                return false;
            }

            if (recorder.TryRecordNamedArgument(namedArgument.Key, namedArgument.Value, syntax) is false)
            {
                return false;
            }
        }

        return true;
    }

    private sealed class SemanticAttributeRecorder : ISemanticAttributeRecorder
    {
        private Dictionary<ITypeParameterSymbol, ITypeSymbol> TypeArgumentsDictionary { get; } = new(SymbolEqualityComparer.Default);
        private Dictionary<IParameterSymbol, object?> ConstructorArgumentsDictionary { get; } = new(SymbolEqualityComparer.Default);
        private Dictionary<string, object?> NamedArgumentsDictionary { get; } = new();

        public IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> TypeArguments => TypeArgumentsDictionary;
        public IReadOnlyDictionary<IParameterSymbol, object?> ConstructorArguments => ConstructorArgumentsDictionary;
        public IReadOnlyDictionary<string, object?> NamedArguments => NamedArgumentsDictionary;

        bool ISemanticAttributeRecorder.TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
        {
            TypeArgumentsDictionary[parameter] = argument;

            return true;
        }

        bool ISemanticAttributeRecorder.TryRecordConstructorArgument(IParameterSymbol parameter, object? argument)
        {
            ConstructorArgumentsDictionary[parameter] = argument;

            return true;
        }

        bool ISemanticAttributeRecorder.TryRecordNamedArgument(string parameterName, object? argument)
        {
            NamedArgumentsDictionary[parameterName] = argument;

            return true;
        }
    }

    private sealed class SyntacticAttributeRecorder : ISyntacticAttributeRecorder
    {
        private Dictionary<ITypeParameterSymbol, ExpressionSyntax> TypeArgumentsDictionary { get; } = new(SymbolEqualityComparer.Default);
        private Dictionary<IParameterSymbol, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> ConstructorArgumentsDictionary { get; } = new(SymbolEqualityComparer.Default);
        private Dictionary<string, ExpressionSyntax> NamedArgumentsDictionary { get; } = new();

        public IReadOnlyDictionary<ITypeParameterSymbol, ExpressionSyntax> TypeArguments => TypeArgumentsDictionary;
        public IReadOnlyDictionary<IParameterSymbol, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> ConstructorArguments => ConstructorArgumentsDictionary;
        public IReadOnlyDictionary<string, ExpressionSyntax> NamedArguments => NamedArgumentsDictionary;

        bool ISyntacticAttributeRecorder.TryRecordTypeArgumentSyntax(ITypeParameterSymbol parameter, ExpressionSyntax syntax)
        {
            TypeArgumentsDictionary[parameter] = syntax;

            return true;
        }

        bool ISyntacticAttributeRecorder.TryRecordConstructorArgumentSyntax(IParameterSymbol parameter, ExpressionSyntax syntax)
        {
            ConstructorArgumentsDictionary[parameter] = syntax;

            return true;
        }

        bool ISyntacticAttributeRecorder.TryRecordConstructorParamsArgumentSyntax(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax)
        {
            ConstructorArgumentsDictionary[parameter] = OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(elementSyntax);

            return true;
        }

        bool ISyntacticAttributeRecorder.TryRecordNamedArgumentSyntax(string parameterName, ExpressionSyntax syntax)
        {
            NamedArgumentsDictionary[parameterName] = syntax;

            return true;
        }
    }
}
