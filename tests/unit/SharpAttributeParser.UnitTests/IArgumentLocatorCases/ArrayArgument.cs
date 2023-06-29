namespace SharpAttributeParser.Tests.IArgumentLocatorCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

public class ArrayArgument
{
    private static (Location Collection, IReadOnlyList<Location> Elements) Target(IArgumentLocator locator, ExpressionSyntax expression) => locator.ArrayArgument(expression);

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var expression = Datasets.GetNullArrayArgumentExpression();

        var exception = Record.Exception(() => Target(locator, expression));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void NullLiteral_CollectionIncludesEntire_ElementsEmpty(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expression.GetLocation(), actualCollection);
        Assert.Empty(actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void DefaultLiteral_CollectionIncludesEntire_ElementsEmpty(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expression.GetLocation(), actualCollection);
        Assert.Empty(actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Implicit_Empty_CollectionIncludesInitializer_ElementsEmpty(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.ImplicitArrayCreationExpression(SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression));

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expression.Initializer.GetLocation(), actualCollection);
        Assert.Empty(actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Implicit_Elements_CollectionIncludesInitializer_ElementsMatchSingleArgument(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.ImplicitArrayCreationExpression(GetArrayInitializer());

        var expectedElementLocations = expression.Initializer.Expressions.Select(locator.SingleArgument);

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expression.Initializer.GetLocation(), actualCollection);
        Assert.Equal(expectedElementLocations, actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Explicit_ExplicitlyEmpty_CollectionIncludesEntire_ElementsEmpty(IArgumentLocator locator)
    {
        var rankSpecifierList = SyntaxFactory.List(new[] { SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SeparatedList(new ExpressionSyntax[] { SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)) })) });

        var expression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("int"), rankSpecifierList));

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expression.GetLocation(), actualCollection);
        Assert.Empty(actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Explicit_Empty_CollectionIncludesInitializer_ElementsEmpty(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("int")), SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression));

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expression.Initializer!.GetLocation(), actualCollection);
        Assert.Empty(actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Explicit_Elements_CollectionIncludesInitializer_ElementsMatchEachArgument(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expectedElementLocations = expression.Initializer!.Expressions.Select(locator.SingleArgument);

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expression.Initializer.GetLocation(), actualCollection);
        Assert.Equal(expectedElementLocations, actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Parenthesis_OnlyIncludesExpression(IArgumentLocator locator)
    {
        var arrayCreationExpression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expression = SyntaxFactory.ParenthesizedExpression(arrayCreationExpression);

        var (expectedCollection, expectedElements) = Target(locator, expression.Expression);

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expectedCollection, actualCollection);
        Assert.Equal(expectedElements, actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void NestedParenthesis_OnlyIncludesNestedExpression(IArgumentLocator locator)
    {
        var arrayCreationExpression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expression = SyntaxFactory.ParenthesizedExpression(SyntaxFactory.ParenthesizedExpression(arrayCreationExpression));

        var (expectedCollection, expectedElements) = Target(locator, expression.Expression);

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expectedCollection, actualCollection);
        Assert.Equal(expectedElements, actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Cast_OnlyIncludesExpression(IArgumentLocator locator)
    {
        var arrayCreationExpression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("object[]"), arrayCreationExpression);

        var (expectedCollection, expectedElements) = Target(locator, expression.Expression);

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expectedCollection, actualCollection);
        Assert.Equal(expectedElements, actualElements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void NestedCast_OnlyIncludesNestedExpression(IArgumentLocator locator)
    {
        var arrayCreationExpression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("object"), SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("object[]"), arrayCreationExpression));

        var (expectedCollection, expectedElements) = Target(locator, expression.Expression);

        var (actualCollection, actualElements) = Target(locator, expression);

        Assert.Equal(expectedCollection, actualCollection);
        Assert.Equal(expectedElements, actualElements);
    }

    private static InitializerExpressionSyntax GetArrayInitializer()
    {
        var stringLiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("42"));

        var nullLiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var typeofExpression = SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName("int"));

        var nameofArguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(SyntaxFactory.ParseTypeName("int")) }));

        var nameofExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("nameof"), nameofArguments);

        var parenthesizedExpression = SyntaxFactory.ParenthesizedExpression(SyntaxFactory.ParenthesizedExpression(stringLiteralExpression));

        var numericLiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42));

        var castExpression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("double"), SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("float"), numericLiteralExpression));

        var arrayExpression = SyntaxFactory.ImplicitArrayCreationExpression(SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression));

        var arguments = SyntaxFactory.SeparatedList(new ExpressionSyntax[] { stringLiteralExpression, nullLiteralExpression, typeofExpression, nameofExpression, parenthesizedExpression, castExpression, arrayExpression });

        return SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, arguments);
    }
}
