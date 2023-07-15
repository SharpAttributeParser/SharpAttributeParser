namespace SharpAttributeParser.ArgumentLocatorCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Linq;

using Xunit;

public sealed class ArrayArgument
{
    private static CollectionLocation Target(IArgumentLocator locator, ExpressionSyntax expression) => locator.ArrayArgument(expression);

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var exception = Record.Exception(() => Target(locator, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NotArray_ArgumentException(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);

        var exception = Record.Exception(() => Target(locator, expression));

        Assert.IsType<ArgumentException>(exception);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NullLiteral_CollectionIncludesEntire_ElementsEmpty(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var expected = CollectionLocation.Empty(expression.GetLocation());

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void DefaultLiteral_CollectionIncludesEntire_ElementsEmpty(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);

        var expected = CollectionLocation.Empty(expression.GetLocation());

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Implicit_Empty_CollectionIncludesInitializer_ElementsEmpty(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.ImplicitArrayCreationExpression(SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression));

        var expected = CollectionLocation.Empty(expression.Initializer.GetLocation());

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Implicit_Elements_CollectionIncludesInitializer_ElementsMatchSingleArgument(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.ImplicitArrayCreationExpression(GetArrayInitializer());

        var expectedCollection = expression.Initializer.GetLocation();
        var expectedElements = expression.Initializer.Expressions.Select(locator.SingleArgument).ToList();

        var expected = CollectionLocation.Create(expectedCollection, expectedElements);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Explicit_ExplicitlyEmpty_CollectionIncludesEntire_ElementsEmpty(IArgumentLocator locator)
    {
        var rankSpecifierList = SyntaxFactory.List(new[] { SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SeparatedList(new ExpressionSyntax[] { SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)) })) });

        var expression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("int"), rankSpecifierList));

        var expected = CollectionLocation.Empty(expression.GetLocation());

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Explicit_Empty_CollectionIncludesInitializer_ElementsEmpty(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("int")), SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression));

        var expected = CollectionLocation.Empty(expression.Initializer!.GetLocation());

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Explicit_Elements_CollectionIncludesInitializer_ElementsMatchEachArgument(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expectedCollection = expression.Initializer!.GetLocation();
        var expectedElements = expression.Initializer!.Expressions.Select(locator.SingleArgument).ToList();

        var expected = CollectionLocation.Create(expectedCollection, expectedElements);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Parenthesis_OnlyIncludesExpression(IArgumentLocator locator)
    {
        var arrayCreationExpression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expression = SyntaxFactory.ParenthesizedExpression(arrayCreationExpression);

        var expected = locator.ArrayArgument(expression.Expression);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NestedParenthesis_OnlyIncludesNestedExpression(IArgumentLocator locator)
    {
        var arrayCreationExpression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expression = SyntaxFactory.ParenthesizedExpression(SyntaxFactory.ParenthesizedExpression(arrayCreationExpression));

        var expected = locator.ArrayArgument(expression.Expression);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Cast_OnlyIncludesExpression(IArgumentLocator locator)
    {
        var arrayCreationExpression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("object[]"), arrayCreationExpression);

        var expected = locator.ArrayArgument(expression.Expression);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NestedCast_OnlyIncludesNestedExpression(IArgumentLocator locator)
    {
        var arrayCreationExpression = SyntaxFactory.ArrayCreationExpression(SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName("object")), GetArrayInitializer());

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("object"), SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("object[]"), arrayCreationExpression));

        var expected = locator.ArrayArgument(expression.Expression);

        IdenticalToExpected(locator, expression, expected);
    }

    [AssertionMethod]
    private static void IdenticalToExpected(IArgumentLocator locator, ExpressionSyntax expression, CollectionLocation expected)
    {
        var actual = Target(locator, expression);

        Assert.Equal(expected.Collection, actual.Collection);
        Assert.Equal(expected.Elements, actual.Elements);
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
