namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;

internal static class Datasets
{
    public static string GetValidParameterName() => "Bar";
    public static string GetNullParameterName() => null!;

    public static Location GetValidLocation() => Location.None;
    public static Location GetNullLocation() => null!;

    public static IReadOnlyList<Location> GetValidElementLocations() => Array.Empty<Location>();
    public static IReadOnlyList<Location> GetNullElementLocations() => null!;

    public static ITypeSymbol GetValidTypeSymbol() => new Mock<ITypeSymbol>().Object;
    public static ITypeSymbol GetNullTypeSymbol() => null!;

    public static IReadOnlyList<object?>? GetValidArrayArgument() => new[] { "42" };
    public static IReadOnlyList<object?>? GetNullArrayArgument() => null;
    public static IReadOnlyList<object?>? GetNullContainingArrayArgument() => new object?[] { null };

    public static IEqualityComparer<string> GetNullComparer() => null!;
}
