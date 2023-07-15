namespace SharpAttributeParser.ArgumentLocatorCases;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class ParamsArguments_Params
{
    private static CollectionLocation Target(IArgumentLocator locator, ExpressionSyntax[] expressions) => locator.ParamsArguments(expressions);

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var exception = Record.Exception(() => Target(locator, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NullContaining_ArgumentNullException(IArgumentLocator locator)
    {
        var expressions = new ExpressionSyntax[]
        {
            SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression),
            null!
        };

        var exception = Record.Exception(() => Target(locator, expressions));

        Assert.IsType<ArgumentException>(exception);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NullLiteral_IdenticalToList(IArgumentLocator locator)
    {
        var expressions = new[] { SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression) };

        IdenticalToList(locator, expressions);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Empty_IdenticalToList(IArgumentLocator locator)
    {
        var expressions = Array.Empty<ExpressionSyntax>();

        IdenticalToList(locator, expressions);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Elements_IdenticalToList(IArgumentLocator locator)
    {
        var expressions = GetExpressions();

        IdenticalToList(locator, expressions);
    }

    [AssertionMethod]
    private static void IdenticalToList(IArgumentLocator locator, ExpressionSyntax[] expressions)
    {
        var expected = locator.ParamsArguments((IReadOnlyList<ExpressionSyntax>)expressions);

        var actual = Target(locator, expressions);

        Assert.Equal(expected.Collection, actual.Collection);
        Assert.Equal(expected.Elements, actual.Elements);
    }

    private static ExpressionSyntax[] GetExpressions()
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

        return new ExpressionSyntax[] { stringLiteralExpression, nullLiteralExpression, typeofExpression, nameofExpression, parenthesizedExpression, castExpression, arrayExpression };
    }
}
