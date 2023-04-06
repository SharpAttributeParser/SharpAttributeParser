namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

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

    public static ITypeSymbol GetValidTypeSymbol() => new Mock<ITypeSymbol>().Object;
    public static ITypeSymbol GetNullTypeSymbol() => null!;

    public static IReadOnlyList<object?>? GetValidArrayArgument() => new[] { "42" };
    public static IReadOnlyList<object?>? GetNullArrayArgument() => null;
    public static IReadOnlyList<object?>? GetNullContainingArrayArgument() => new object?[] { null };

    public static IEqualityComparer<string> GetNullComparer() => null!;
}
