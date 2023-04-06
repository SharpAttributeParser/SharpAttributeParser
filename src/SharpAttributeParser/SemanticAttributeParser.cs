namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Linq;

/// <inheritdoc cref="ISemanticAttributeParser"/>
public sealed class SemanticAttributeParser : ISemanticAttributeParser
{
    /// <inheritdoc/>
    public bool TryParse(ISemanticArgumentRecorder recorder, AttributeData attributeData)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        if (attributeData is null)
        {
            throw new ArgumentNullException(nameof(attributeData));
        }

        if (attributeData.AttributeClass is null || attributeData.AttributeClass.TypeKind is TypeKind.Error || attributeData.AttributeConstructor is null)
        {
            return false;
        }

        if (TryParseGenericArguments(recorder, attributeData) is false)
        {
            return false;
        }

        if (TryParseConstructorArguments(recorder, attributeData) is false)
        {
            return false;
        }

        if (TryParseNamedArguments(recorder, attributeData) is false)
        {
            return false;
        }

        return true;
    }

    private static bool TryParseGenericArguments(ISemanticArgumentRecorder recorder, AttributeData attributeData)
    {
        if (attributeData.AttributeClass is not INamedTypeSymbol attributeType)
        {
            return false;
        }

        for (var i = 0; i < attributeType.TypeArguments.Length; i++)
        {
            if (recorder.TryRecordGenericArgument(attributeType.TypeParameters[i], attributeType.TypeArguments[i]) is false)
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryParseConstructorArguments(ISemanticArgumentRecorder recorder, AttributeData attributeData)
    {
        if (attributeData.AttributeConstructor is not IMethodSymbol attributeConstructor)
        {
            return false;
        }

        if (attributeData.ConstructorArguments.IsEmpty)
        {
            return true;
        }

        if (attributeData.ConstructorArguments.Length < attributeConstructor.Parameters.Count())
        {
            return false;
        }

        for (var i = 0; i < attributeConstructor.Parameters.Count() && i < attributeData.ConstructorArguments.Length; i++)
        {
            if (attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Error)
            {
                continue;
            }

            if (i == attributeConstructor.Parameters.Count() - 1 && attributeConstructor.Parameters.Count() < attributeData.ConstructorArguments.Length)
            {
                if (recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[i], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[i])) is false)
                {
                    return false;
                }

                break;
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

    private static bool TryParseNamedArguments(ISemanticArgumentRecorder recorder, AttributeData attributeData)
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
