namespace SharpAttributeParser.Tests.IArgumentLocatorCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

public class ParamsArguments_Params
{
    private static (Location Collection, IReadOnlyList<Location> Elements) Target(IArgumentLocator locator, ExpressionSyntax[] expressions) => locator.ParamsArguments(expressions);

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var expressions = Datasets.GetNullParamsArgumentExpression();

        var exception = Record.Exception(() => Target(locator, expressions));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void NullContaining_ArgumentNullException(IArgumentLocator locator)
    {
        var expressions = Datasets.GetNullContainingParamsArgumentExpression();

        var exception = Record.Exception(() => Target(locator, expressions));

        Assert.IsType<ArgumentException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void NullLiteral_CollectionIncludesEntire_ElementsContainsNull(IArgumentLocator locator)
    {
        var expressions = new[] { SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression) };

        var expectedElementLocations = expressions.Select(locator.SingleArgument);

        var (collection, elements) = Target(locator, expressions);

        Assert.Equal(expressions[0].GetLocation(), collection);
        Assert.Equal(expectedElementLocations, elements);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void CollectionIncludesEntire_ElementsMatchEachArgument(IArgumentLocator locator)
    {
        var expressions = GetExpressions();

        var expectedElementLocations = expressions.Select(locator.SingleArgument);

        var (collection, elements) = Target(locator, expressions);

        Assert.Equal(expandLocation(expressions[0].GetLocation(), expressions[^1].GetLocation()), collection);
        Assert.Equal(expectedElementLocations, elements);

        static Location expandLocation(Location first, Location second)
        {
            var textSpan = TextSpan.FromBounds(first.SourceSpan.Start, second.SourceSpan.End);

            return Location.Create(first.SourceTree!, textSpan);
        }
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
