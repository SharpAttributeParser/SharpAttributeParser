namespace SharpAttributeParser.ArgumentLocatorCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

public sealed class ParamsArguments_IReadOnlyList
{
    private static CollectionLocation Target(IArgumentLocator locator, IReadOnlyList<ExpressionSyntax> expressions) => locator.ParamsArguments(expressions);

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var exception = Record.Exception(() => Target(locator, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NullContaining_ArgumentException(IArgumentLocator locator)
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
    public void NullLiteral_CollectionIncludesEntire_ElementsContainsNull(IArgumentLocator locator)
    {
        var expressions = new[] { SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression) };

        IdenticalToExpected(locator, expressions);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Empty_CollectionNoLocation_ElementsEmpty(IArgumentLocator locator)
    {
        var expressions = Array.Empty<ExpressionSyntax>();

        IdenticalToExpected(locator, expressions);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Elements_CollectionIncludesEntire_ElementsMatchEachArgument(IArgumentLocator locator)
    {
        var expressions = GetExpressions();

        IdenticalToExpected(locator, expressions);
    }

    [AssertionMethod]
    private static void IdenticalToExpected(IArgumentLocator locator, IReadOnlyList<ExpressionSyntax> expressions)
    {
        var expected = getExpected();

        var actual = Target(locator, expressions);

        Assert.Equal(expected.Collection, actual.Collection);
        Assert.Equal(expected.Elements, actual.Elements);

        CollectionLocation getExpected()
        {
            if (expressions.Count is 0)
            {
                return CollectionLocation.None;
            }

            var textSpan = TextSpan.FromBounds(expressions[0].GetLocation().SourceSpan.Start, expressions[^1].GetLocation().SourceSpan.End);

            var expectedCollection = Location.Create(expressions[0].GetLocation().SourceTree!, textSpan);
            var expectedElements = expressions.Select(locator.SingleArgument).ToList();

            return CollectionLocation.Create(expectedCollection, expectedElements);
        }
    }

    private static IReadOnlyList<ExpressionSyntax> GetExpressions()
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
