namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using System.Collections.Generic;

internal static class Datasets
{
    public static string GetValidParameterName() => "Bar";
    public static string GetNullParameterName() => null!;

    public static ITypeParameterSymbol GetMockedTypeParameter() => Mock.Of<ITypeParameterSymbol>(static (parameter) => parameter.Name == "Bar");
    public static ITypeParameterSymbol GetNullTypeParameter() => null!;

    public static IParameterSymbol GetMockedParameter() => Mock.Of<IParameterSymbol>(static (parameter) => parameter.Name == "Bar");
    public static IParameterSymbol GetNullParameter() => null!;

    public static ITypeSymbol GetValidTypeSymbol() => Mock.Of<ITypeSymbol>();
    public static ITypeSymbol GetNullTypeSymbol() => null!;

    public static IReadOnlyList<object?>? GetValidArrayArgument() => new[] { "42" };
    public static IReadOnlyList<object?>? GetNullArrayArgument() => null;
    public static IReadOnlyList<object?>? GetNullContainingArrayArgument() => new object?[] { null };

    public static IEqualityComparer<string> GetNullComparer() => null!;
}
