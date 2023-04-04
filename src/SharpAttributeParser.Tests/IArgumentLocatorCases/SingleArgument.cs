namespace SharpAttributeParser.Tests.IArgumentLocatorCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

using Xunit;

public class SingleArgument
{
    private static Location Target(IArgumentLocator locator, ExpressionSyntax expression) => locator.SingleArgument(expression);

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Null_ArgumentNullException(IArgumentLocator locator)
    {
        var expression = Datasets.GetNullSingleArgumentExpression();

        var exception = Record.Exception(() => Target(locator, expression));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void String_IncludesEntire(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("42"));

        var actual = Target(locator, expression);

        Assert.Equal(expression.GetLocation(), actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Typeof_OnlyIncludesType(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName("int"));

        var actual = Target(locator, expression);

        Assert.Equal(expression.Type.GetLocation(), actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Nameof_IncludesEntire(IArgumentLocator locator)
    {
        var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(SyntaxFactory.ParseTypeName("int")) }));

        var expression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("nameof"), arguments);

        var actual = Target(locator, expression);

        Assert.Equal(expression.GetLocation(), actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void NullLiteral_IncludesEntire(IArgumentLocator locator)
    {
        var expression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        var actual = Target(locator, expression);

        Assert.Equal(expression.GetLocation(), actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Parenthesis_OnlyIncludesExpression(IArgumentLocator locator)
    {
        var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("42"));

        var expression = SyntaxFactory.ParenthesizedExpression(literalExpression);

        var expected = Target(locator, expression.Expression);

        var actual = Target(locator, expression);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void NestedParenthesis_OnlyIncludesNestedExpression(IArgumentLocator locator)
    {
        var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("42"));

        var expression = SyntaxFactory.ParenthesizedExpression(SyntaxFactory.ParenthesizedExpression(literalExpression));

        var expected = Target(locator, expression.Expression);

        var actual = Target(locator, expression);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Cast_OnlyIncludesExpression(IArgumentLocator locator)
    {
        var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42));

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("double"), literalExpression);

        var expected = Target(locator, expression.Expression);

        var actual = Target(locator, expression);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void NestedCast_OnlyIncludesNestedExpression(IArgumentLocator locator)
    {
        var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(42));

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("double"), SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("float"), literalExpression));

        var expected = Target(locator, expression.Expression);

        var actual = Target(locator, expression);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Operator_IncludesFullOperation(IArgumentLocator locator)
    {
        var leftiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(41));
        var rightLiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));

        var expression = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, leftiteralExpression, rightLiteralExpression);

        var actual = Target(locator, expression);

        Assert.Equal(expression.GetLocation(), actual);
    }

    [Theory]
    [ClassData(typeof(Datasets.Sources))]
    public void Combined_OnlyIncludesSimplified(IArgumentLocator locator)
    {
        var typeofExpression = SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName("int"));

        var expression = SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName("Type"), typeofExpression);

        var actual = Target(locator, expression);

        Assert.Equal(((TypeOfExpressionSyntax)expression.Expression).Type.GetLocation(), actual);
    }
}
