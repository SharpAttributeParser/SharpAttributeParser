namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        return TryParse(recorder, attributeData, attributeType, targetConstructor);
    }

    private static bool TryParse(ISemanticAttributeRecorder recorder, AttributeData attributeData, INamedTypeSymbol attributeType, IMethodSymbol targetConstructor)
    {
        return TryParseGenericArguments(recorder, attributeType) && TryParseConstructorArguments(recorder, attributeData, targetConstructor) && TryParseNamedArguments(recorder, attributeData);
    }

    private static bool TryParseGenericArguments(ISemanticAttributeRecorder recorder, INamedTypeSymbol attributeType)
    {
        if (attributeType.TypeParameters.Length != attributeType.TypeArguments.Length)
        {
            return false;
        }

        return Enumerable.Range(0, attributeType.TypeParameters.Length).All((index) => TryParseGenericArgument(recorder, attributeType.TypeParameters[index], attributeType.TypeArguments[index]));
    }

    private static bool TryParseGenericArgument(ISemanticAttributeRecorder recorder, ITypeParameterSymbol parameter, ITypeSymbol argument)
    {
        if (argument.Kind is SymbolKind.ErrorType)
        {
            return false;
        }

        return recorder.TryRecordTypeArgument(parameter, argument);
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

        return Enumerable.Range(0, attributeData.ConstructorArguments.Length).All((index) => TryParseConstructorArgument(recorder, attributeConstructor.Parameters[index], attributeData.ConstructorArguments[index]));
    }

    private static bool TryParseConstructorArgument(ISemanticAttributeRecorder recorder, IParameterSymbol parameter, TypedConstant argument)
    {
        if (argument.Kind is TypedConstantKind.Error)
        {
            return false;
        }

        if (argument.Kind is TypedConstantKind.Array)
        {
            return recorder.TryRecordConstructorArgument(parameter, ParseArrayArguments(argument));
        }

        return recorder.TryRecordConstructorArgument(parameter, ParseSingleArgument(argument));
    }

    private static bool TryParseNamedArguments(ISemanticAttributeRecorder recorder, AttributeData attributeData)
    {
        if (attributeData.NamedArguments.IsEmpty)
        {
            return true;
        }

        return Enumerable.Range(0, attributeData.NamedArguments.Length).All((index) => TryParseNamedArgument(recorder, attributeData.NamedArguments[index]));
    }

    private static bool TryParseNamedArgument(ISemanticAttributeRecorder recorder, KeyValuePair<string, TypedConstant> argument)
    {
        if (argument.Value.Kind is TypedConstantKind.Error)
        {
            return true;
        }

        if (argument.Value.Kind is TypedConstantKind.Array)
        {
            return recorder.TryRecordNamedArgument(argument.Key, ParseArrayArguments(argument.Value));
        }

        return recorder.TryRecordNamedArgument(argument.Key, ParseSingleArgument(argument.Value));
    }

    private static object? ParseSingleArgument(TypedConstant value) => value.Value;

    [SuppressMessage("Major Code Smell", "S1168: Empty arrays and collections should be returned instead of null", Justification = "User-provided value was null.")]
    private static object?[]? ParseArrayArguments(TypedConstant value)
    {
        if (value.IsNull)
        {
            return null;
        }

        if (value.Values.IsEmpty)
        {
            return Array.Empty<object?>();
        }

        var arrayConstants = value.Values;
        var arrayValues = new object?[arrayConstants.Length];

        for (var i = 0; i < arrayConstants.Length; i++)
        {
            if (arrayConstants[i].Kind is TypedConstantKind.Array)
            {
                arrayValues[i] = ParseArrayArguments(arrayConstants[i]);

                continue;
            }

            arrayValues[i] = arrayConstants[i].Value;
        }

        return arrayValues;
    }
}
