namespace SharpAttributeParser.ArgumentLocatorCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

using Xunit;

public sealed class SingleArgument
{
    private static Location Target(IArgumentLocator locator, ExpressionSyntax expression) => locator.SingleArgument(expression);

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var exception = Record.Exception(() => Target(locator, null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void String_IncludesEntire(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("42"));

        var expected = expression.GetLocation();

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Typeof_OnlyIncludesType(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName("int"));

        var expected = expression.Type.GetLocation();

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Nameof_IncludesEntire(IArgumentLocator locator)
    {
        var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(SyntaxFactory.ParseTypeName("int")) }));

        var expression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("nameof"), arguments);

        var expected = expression.GetLocation();

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NullLiteral_IncludesEntire(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var expected = expression.GetLocation();

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Parenthesis_OnlyIncludesExpression(IArgumentLocator locator)
    {
        var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("42"));

        var expression = SyntaxFactory.ParenthesizedExpression(literalExpression);

        var expected = locator.SingleArgument(expression.Expression);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NestedParenthesis_OnlyIncludesNestedExpression(IArgumentLocator locator)
    {
        var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("42"));

        var expression = SyntaxFactory.ParenthesizedExpression(SyntaxFactory.ParenthesizedExpression(literalExpression));

        var expected = locator.SingleArgument(expression.Expression);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Cast_OnlyIncludesExpression(IArgumentLocator locator)
    {
        var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42));

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("double"), literalExpression);

        var expected = locator.SingleArgument(expression.Expression);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void NestedCast_OnlyIncludesNestedExpression(IArgumentLocator locator)
    {
        var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42));

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("double"), SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("float"), literalExpression));

        var expected = locator.SingleArgument(expression.Expression);

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Operator_IncludesFullOperation(IArgumentLocator locator)
    {
        var leftiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(41));
        var rightLiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));

        var expression = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, leftiteralExpression, rightLiteralExpression);

        var expected = expression.GetLocation();

        IdenticalToExpected(locator, expression, expected);
    }

    [Theory]
    [ClassData(typeof(LocatorSources))]
    public void Combined_OnlyIncludesSimplified(IArgumentLocator locator)
    {
        var typeofExpression = SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName("int"));

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("Type"), typeofExpression);

        var expected = ((TypeOfExpressionSyntax)expression.Expression).Type.GetLocation();

        IdenticalToExpected(locator, expression, expected);
    }

    [AssertionMethod]
    private static void IdenticalToExpected(IArgumentLocator locator, ExpressionSyntax expression, Location expected)
    {
        var actual = Target(locator, expression);

        Assert.Equivalent(expected, actual);
    }
}
