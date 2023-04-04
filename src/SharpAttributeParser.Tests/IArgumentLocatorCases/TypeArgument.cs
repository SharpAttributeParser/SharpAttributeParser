namespace SharpAttributeParser.Tests.IArgumentLocatorCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

using Xunit;

public class TypeArgument
{
    private static Location Target(IArgumentLocator locator, TypeSyntax type) => locator.TypeArgument(type);

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var type = Datasets.GetNullTypeArgument();

        var exception = Record.Exception(() => Target(locator, type));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Valid_MatchOriginal(IArgumentLocator locator)
    {
        var type = SyntaxFactory.ParseTypeName("int");

        var actual = Target(locator, type);

        Assert.Equal(type.GetLocation(), actual);
    }
}
