namespace SharpAttributeParser.ArgumentLocatorCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

using Xunit;

public sealed class TypeArgument
{
    private static Location Target(IArgumentLocator locator, TypeSyntax type) => locator.TypeArgument(type);

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var exception = Record.Exception(() => Target(locator, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Valid_MatchOriginal(IArgumentLocator locator)
    {
        var syntax = SyntaxFactory.ParseTypeName("int");

        var expected = syntax.GetLocation();

        var actual = Target(locator, syntax);

        Assert.Equal(expected, actual);
    }
}
