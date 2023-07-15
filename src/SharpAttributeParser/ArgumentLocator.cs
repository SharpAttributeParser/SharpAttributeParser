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
    public CollectionLocation ArrayArgument(ExpressionSyntax expression)
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

            return CollectionLocation.Empty(arrayCreationExpression.GetLocation());
        }

        if (expression is ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression)
        {
            return FromArray(implicitArrayCreationExpression.Initializer);
        }

        if (expression is CastExpressionSyntax castExpression)
        {
            return ArrayArgument(castExpression.Expression);
        }

        if (expression is LiteralExpressionSyntax && (expression.IsKind(SyntaxKind.NullLiteralExpression) || expression.IsKind(SyntaxKind.DefaultLiteralExpression)))
        {
            return CollectionLocation.Empty(expression.GetLocation());
        }

        if (expression is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            return ArrayArgument(parenthesizedExpression.Expression);
        }

        if (expression is DefaultExpressionSyntax)
        {
            return CollectionLocation.Empty(expression.GetLocation());
        }

        throw new ArgumentException($"The provided {nameof(ExpressionSyntax)} could not be interpreted as constructing an array.", nameof(expression));
    }

    /// <inheritdoc/>
    public CollectionLocation ParamsArguments(IReadOnlyList<ExpressionSyntax> expressions)
    {
        if (expressions is null)
        {
            throw new ArgumentNullException(nameof(expressions));
        }

        if (expressions.Count is 0)
        {
            return CollectionLocation.None;
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

        return CollectionLocation.Create(collectionLocation, elementLocations);
    }

    /// <inheritdoc/>
    public CollectionLocation ParamsArguments(params ExpressionSyntax[] expressions) => ParamsArguments(expressions as IReadOnlyList<ExpressionSyntax>);

    private CollectionLocation FromArray(InitializerExpressionSyntax initializerExpression)
    {
        var elementLocations = new Location[initializerExpression.Expressions.Count];

        for (var i = 0; i < elementLocations.Length; i++)
        {
            elementLocations[i] = SingleArgument(initializerExpression.Expressions[i]);
        }

        return CollectionLocation.Create(initializerExpression.GetLocation(), elementLocations);
    }

    private static Location CreateCollectionLocation(Location firstLocation, Location lastLocation)
    {
        var textSpan = TextSpan.FromBounds(firstLocation.SourceSpan.Start, lastLocation.SourceSpan.End);

        return Location.Create(firstLocation.SourceTree!, textSpan);
    }
}
