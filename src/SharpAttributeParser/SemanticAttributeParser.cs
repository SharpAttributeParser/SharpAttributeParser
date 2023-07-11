﻿namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Linq;

/// <inheritdoc cref="ISemanticAttributeParser"/>
public sealed class SemanticAttributeParser : ISemanticAttributeParser
{
    /// <inheritdoc/>
    public bool TryParse(ISemanticAttributeRecorder recorder, AttributeData attributeData)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        if (attributeData is null)
        {
            throw new ArgumentNullException(nameof(attributeData));
        }

        if (attributeData.AttributeClass is not INamedTypeSymbol attributeType || attributeType.TypeKind is TypeKind.Error || attributeData.AttributeConstructor is not IMethodSymbol targetConstructor)
        {
            return false;
        }

        return TryParseGenericArguments(recorder, attributeType) && TryParseConstructorArguments(recorder, attributeData, targetConstructor) && TryParseNamedArguments(recorder, attributeData);
    }

    private static bool TryParseGenericArguments(ISemanticAttributeRecorder recorder, INamedTypeSymbol attributeType)
    {
        if (attributeType.TypeParameters.Length != attributeType.TypeArguments.Length)
        {
            return false;
        }

        for (var i = 0; i < attributeType.TypeArguments.Length; i++)
        {
            if (attributeType.TypeArguments[i].Kind is SymbolKind.ErrorType)
            {
                return false;
            }

            if (recorder.TryRecordTypeArgument(attributeType.TypeParameters[i], attributeType.TypeArguments[i]) is false)
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryParseConstructorArguments(ISemanticAttributeRecorder recorder, AttributeData attributeData, IMethodSymbol attributeConstructor)
    {
        if (attributeData.ConstructorArguments.IsEmpty)
        {
            return true;
        }

        if (attributeData.ConstructorArguments.Length != attributeConstructor.Parameters.Count())
        {
            return false;
        }

        for (var i = 0; i < attributeConstructor.Parameters.Count() && i < attributeData.ConstructorArguments.Length; i++)
        {
            if (attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Error)
            {
                return false;
            }

            if (attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Array)
            {
                if (recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[i], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[i])) is false)
                {
                    return false;
                }

                continue;
            }

            if (recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[i], CommonAttributeParsing.SingleArgument(attributeData.ConstructorArguments[i])) is false)
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryParseNamedArguments(ISemanticAttributeRecorder recorder, AttributeData attributeData)
    {
        if (attributeData.NamedArguments.IsEmpty)
        {
            return true;
        }

        for (var i = 0; i < attributeData.NamedArguments.Length; i++)
        {
            if (attributeData.NamedArguments[i].Value.Kind is TypedConstantKind.Error)
            {
                continue;
            }

            if (attributeData.NamedArguments[i].Value.Kind is TypedConstantKind.Array)
            {
                if (recorder.TryRecordNamedArgument(attributeData.NamedArguments[i].Key, CommonAttributeParsing.ArrayArguments(attributeData.NamedArguments[i].Value)) is false)
                {
                    return false;
                }

                continue;
            }

            if (recorder.TryRecordNamedArgument(attributeData.NamedArguments[i].Key, CommonAttributeParsing.SingleArgument(attributeData.NamedArguments[i].Value)) is false)
            {
                return false;
            }
        }

        return true;
    }
}
