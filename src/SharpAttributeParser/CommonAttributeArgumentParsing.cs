namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>Provides common functionality related to parsing attribute arguments.</summary>
internal static class CommonAttributeArgumentParsing
{
    /// <summary>Parses the value of a single-valued argument.</summary>
    /// <param name="value">The raw value of the argument.</param>
    /// <returns>The parsed value of the argument.</returns>
    public static object? SingleArgument(TypedConstant value) => value.Value;

    /// <summary>Parses the values of an array-valued argument.</summary>
    /// <param name="value">The raw value of the argument.</param>
    /// <returns>The parsed value of the argument.</returns>
    [SuppressMessage("Major Code Smell", "S1168: Empty arrays and collections should be returned instead of null", Justification = "User-provided value was null.")]
    public static object?[]? ArrayArguments(TypedConstant value)
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
                arrayValues[i] = ArrayArguments(arrayConstants[i]);

                continue;
            }

            arrayValues[i] = arrayConstants[i].Value;
        }

        return arrayValues;
    }
}
