namespace SharpAttributeParser.Tests.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;
using System.Collections.Generic;

internal static class Datasets
{
    public static string GetValidParameterName() => "Bar";
    public static string GetNullParameterName() => null!;

    public static ITypeParameterSymbol GetMockedTypeParameter()
    {
        Mock<ITypeParameterSymbol> mock = new();

        mock.SetupGet(static (parameter) => parameter.Name).Returns("Bar");

        return mock.Object;
    }

    public static ITypeParameterSymbol GetNullTypeParameter() => null!;

    public static IParameterSymbol GetMockedParameter()
    {
        Mock<IParameterSymbol> mock = new();

        mock.SetupGet(static (parameter) => parameter.Name).Returns("Bar");

        return mock.Object;
    }

    public static IParameterSymbol GetNullParameter() => null!;

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
