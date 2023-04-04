namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IArgumentLocator"/>
public sealed class ArgumentLocator : IArgumentLocator
{
    /// <inheritdoc/>
    public Location TypeArgument(TypeSyntax type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return type.GetLocation();
    }

    /// <inheritdoc/>
    public Location SingleArgument(ExpressionSyntax expression)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (expression is TypeOfExpressionSyntax typeofExpression)
        {
            return typeofExpression.Type.GetLocation();
        }

        if (expression is CastExpressionSyntax castExpression)
        {
            return SingleArgument(castExpression.Expression);
        }

        if (expression is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            return SingleArgument(parenthesizedExpression.Expression);
        }

        return expression.GetLocation();
    }

    /// <inheritdoc/>
    public (Location, IReadOnlyList<Location>) ArrayArgument(ExpressionSyntax expression)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (expression is ArrayCreationExpressionSyntax arrayCreationExpression)
        {
            if (arrayCreationExpression.Initializer is not null)
            {
                return FromArray(arrayCreationExpression.Initializer);
            }

            return (arrayCreationExpression.GetLocation(), Array.Empty<Location>());
        }

        if (expression is InitializerExpressionSyntax initializerExpression)
        {
            if (initializerExpression.IsKind(SyntaxKind.ArrayInitializerExpression) is false)
            {
                return (initializerExpression.GetLocation(), Array.Empty<Location>());
            }

            return FromArray(initializerExpression);
        }

        if (expression is ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression)
        {
            return FromArray(implicitArrayCreationExpression.Initializer);
        }

        if (expression is CastExpressionSyntax castExpression)
        {
            return ArrayArgument(castExpression.Expression);
        }

        if (expression is LiteralExpressionSyntax && expression.IsKind(SyntaxKind.NullLiteralExpression))
        {
            return (expression.GetLocation(), Array.Empty<Location>());
        }

        if (expression is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            return ArrayArgument(parenthesizedExpression.Expression);
        }

        throw new ArgumentException($"The provided {nameof(ExpressionSyntax)} could not be interpreted as constructing an array.", nameof(expression));
    }

    /// <inheritdoc/>
    public (Location, IReadOnlyList<Location>) ParamsArguments(IReadOnlyList<ExpressionSyntax> expressions)
    {
        if (expressions is null)
        {
            throw new ArgumentNullException(nameof(expressions));
        }

        var elementLocations = new Location[expressions.Count];

        for (var i = 0; i < elementLocations.Length; i++)
        {
            if (expressions[i] is null)
            {
                throw new ArgumentException($"An element of the provided collection was null, at index {i}.", nameof(expressions));
            }

            elementLocations[i] = SingleArgument(expressions[i]);
        }

        var firstLocation = expressions[0].GetLocation();
        var lastLocation = expressions[expressions.Count - 1].GetLocation();

        var collectionLocation = CreateCollectionLocation(firstLocation, lastLocation);

        return (collectionLocation, elementLocations);
    }

    /// <inheritdoc/>
    public (Location, IReadOnlyList<Location>) ParamsArguments(params ExpressionSyntax[] expressions) => ParamsArguments(expressions as IReadOnlyList<ExpressionSyntax>);

    private (Location, IReadOnlyList<Location>) FromArray(InitializerExpressionSyntax initializerExpression)
    {
        var elementLocations = new Location[initializerExpression.Expressions.Count];

        for (var i = 0; i < elementLocations.Length; i++)
        {
            elementLocations[i] = SingleArgument(initializerExpression.Expressions[i]);
        }

        return new(initializerExpression.GetLocation(), elementLocations);
    }

    private static Location CreateCollectionLocation(Location firstLocation, Location lastLocation)
    {
        var textSpan = TextSpan.FromBounds(firstLocation.SourceSpan.Start, lastLocation.SourceSpan.End);

        return Location.Create(firstLocation.SourceTree!, textSpan);
    }
}
