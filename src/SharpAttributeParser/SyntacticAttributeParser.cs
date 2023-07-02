namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Linq;

/// <inheritdoc cref="ISyntacticAttributeParser"/>
public sealed class SyntacticAttributeParser : ISyntacticAttributeParser
{
    private IArgumentLocator ArgumentLocator { get; }

    /// <summary>Instantiates an <see cref="SyntacticAttributeParser"/>, parsing the arguments of an attribute syntactically.</summary>
    /// <param name="argumentLocator"><inheritdoc cref="IArgumentLocator" path="/summary"/></param>
    /// <exception cref="ArgumentNullException"/>
    public SyntacticAttributeParser(IArgumentLocator argumentLocator)
    {
        ArgumentLocator = argumentLocator ?? throw new ArgumentNullException(nameof(argumentLocator));
    }

    /// <inheritdoc/>
    public bool TryParse(ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        if (attributeData is null)
        {
            throw new ArgumentNullException(nameof(attributeData));
        }

        if (attributeSyntax is null)
        {
            throw new ArgumentNullException(nameof(attributeSyntax));
        }

        if (attributeData.AttributeClass is not INamedTypeSymbol attributeType || attributeType.TypeKind is TypeKind.Error || attributeData.AttributeConstructor is not IMethodSymbol targetConstructor)
        {
            return false;
        }

        if (TryParseGenericArguments(recorder, attributeType, attributeSyntax) is false)
        {
            return false;
        }

        if (TryParseConstructorArguments(recorder, attributeData, targetConstructor, attributeSyntax) is false)
        {
            return false;
        }

        if (TryParseNamedArguments(recorder, attributeData, attributeSyntax) is false)
        {
            return false;
        }

        return true;
    }

    private bool TryParseGenericArguments(ISyntacticArgumentRecorder recorder, INamedTypeSymbol attributeType, AttributeSyntax attributeSyntax)
    {
        if (getGenericNameSyntax() is not GenericNameSyntax genericNameSyntax)
        {
            return true;
        }

        if (genericNameSyntax.TypeArgumentList.Arguments.Count != attributeType.TypeArguments.Length)
        {
            return false;
        }

        for (var i = 0; i < attributeType.TypeArguments.Length; i++)
        {
            if (attributeType.TypeArguments[i].Kind is SymbolKind.ErrorType)
            {
                return false;
            }

            var location = ArgumentLocator.TypeArgument(genericNameSyntax.TypeArgumentList.Arguments[i]);

            if (recorder.TryRecordGenericArgument(attributeType.TypeParameters[i], attributeType.TypeArguments[i], location) is false)
            {
                return false;
            }
        }

        return true;

        GenericNameSyntax? getGenericNameSyntax()
        {
            if (attributeSyntax.Name is GenericNameSyntax genericNameSyntax)
            {
                return genericNameSyntax;
            }

            if (attributeSyntax.Name is QualifiedNameSyntax qualifiedNameSyntax && qualifiedNameSyntax.Right is GenericNameSyntax qualifiedGenericNameSyntax)
            {
                return qualifiedGenericNameSyntax;
            }

            return null;
        }
    }

    private bool TryParseConstructorArguments(ISyntacticArgumentRecorder recorder, AttributeData attributeData, IMethodSymbol targetConstructor, AttributeSyntax attributeSyntax)
    {
        if (attributeData.ConstructorArguments.IsEmpty)
        {
            return true;
        }

        if (attributeData.ConstructorArguments.Length != targetConstructor.Parameters.Count())
        {
            return false;
        }

        if (attributeSyntax.ArgumentList is null)
        {
            if (targetConstructor.Parameters.Length is 1 && targetConstructor.Parameters[0].IsParams)
            {
                var paramsLocation = ArgumentLocator.ParamsArguments(Array.Empty<ExpressionSyntax>());

                return recorder.TryRecordConstructorArgument(targetConstructor.Parameters[0], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[0]), paramsLocation);
            }

            return false;
        }

        for (var i = 0; i < attributeData.ConstructorArguments.Length; i++)
        {
            if (attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Error)
            {
                return false;
            }

            if (targetConstructor.Parameters[i].IsParams)
            {
                if (attributeSyntax.ArgumentList.Arguments.Count <= i || attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Array)
                {
                    return TryParseParamsOrArrayArgument(recorder, attributeData, targetConstructor, attributeSyntax);
                }

                return false;
            }

            if (attributeSyntax.ArgumentList.Arguments.Count <= i)
            {
                return false;
            }

            if (attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Array)
            {
                var collectionLocation = ArgumentLocator.ArrayArgument(attributeSyntax.ArgumentList.Arguments[i].Expression);

                if (recorder.TryRecordConstructorArgument(targetConstructor.Parameters[i], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[i]), collectionLocation) is false)
                {
                    return false;
                }

                continue;
            }

            var location = ArgumentLocator.SingleArgument(attributeSyntax.ArgumentList.Arguments[i].Expression);

            if (recorder.TryRecordConstructorArgument(targetConstructor.Parameters[i], CommonAttributeParsing.SingleArgument(attributeData.ConstructorArguments[i]), location) is false)
            {
                return false;
            }
        }

        return true;
    }

    private bool TryParseParamsOrArrayArgument(ISyntacticArgumentRecorder recorder, AttributeData attributeData, IMethodSymbol targetConstructor, AttributeSyntax attributeSyntax)
    {
        if (attributeSyntax.ArgumentList is null)
        {
            return false;
        }

        var index = attributeData.ConstructorArguments.Length - 1;

        if (attributeData.ConstructorArguments[index].IsNull)
        {
            var nullLocation = ArgumentLocator.ArrayArgument(attributeSyntax.ArgumentList.Arguments[index].Expression);

            return tryRecordConstructorArgument(nullLocation);
        }

        if (attributeSyntax.ArgumentList.Arguments.Count < targetConstructor.Parameters.Length || attributeSyntax.ArgumentList.Arguments[index].NameEquals is not null)
        {
            var emptyParamsLocation = ArgumentLocator.ParamsArguments(Array.Empty<ExpressionSyntax>());

            return tryRecordConstructorArgument(emptyParamsLocation);
        }

        if (attributeSyntax.ArgumentList.Arguments.Count > targetConstructor.Parameters.Length && attributeSyntax.ArgumentList.Arguments[index + 1].NameEquals is null)
        {
            var paramsLocation = ArgumentLocator.ParamsArguments(attributeSyntax.ArgumentList.Arguments.Skip(index).Take(attributeSyntax.ArgumentList.Arguments.Count - (attributeData.ConstructorArguments.Length + attributeData.NamedArguments.Length) + 1).Select(static (argument) => argument.Expression).ToArray());

            return tryRecordConstructorArgument(paramsLocation);
        }

        if (attributeSyntax.ArgumentList.Arguments.Count == targetConstructor.Parameters.Length || attributeSyntax.ArgumentList.Arguments[index + 1].NameEquals is not null)
        {
            if (CheckIfArgumentMatchesArrayStructure(attributeData.ConstructorArguments[index], attributeSyntax.ArgumentList.Arguments[index].Expression))
            {
                var arrayLocation = ArgumentLocator.ArrayArgument(attributeSyntax.ArgumentList.Arguments[index].Expression);

                return tryRecordConstructorArgument(arrayLocation);
            }

            var paramsLocation = ArgumentLocator.ParamsArguments(attributeSyntax.ArgumentList.Arguments[index].Expression);

            return tryRecordConstructorArgument(paramsLocation);
        }

        return false;

        bool tryRecordConstructorArgument(CollectionLocation location) => recorder.TryRecordConstructorArgument(targetConstructor.Parameters[index], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[index]), location);
    }

    private bool TryParseNamedArguments(ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (attributeData.NamedArguments.IsEmpty)
        {
            return true;
        }

        if (attributeSyntax.ArgumentList is null)
        {
            return false;
        }

        for (var i = 0; i < attributeData.NamedArguments.Length; i++)
        {
            if (attributeData.NamedArguments[i].Value.Kind is TypedConstantKind.Error)
            {
                continue;
            }

            if (TryGetExpressionForNamedArgument(attributeData, attributeSyntax, i) is not ExpressionSyntax argumentExpression)
            {
                return false;
            }

            if (attributeData.NamedArguments[i].Value.Kind is TypedConstantKind.Array)
            {
                var collectionLocation = ArgumentLocator.ArrayArgument(argumentExpression);

                if (recorder.TryRecordNamedArgument(attributeData.NamedArguments[i].Key, CommonAttributeParsing.ArrayArguments(attributeData.NamedArguments[i].Value), collectionLocation) is false)
                {
                    return false;
                }

                continue;
            }

            var location = ArgumentLocator.SingleArgument(argumentExpression);

            if (recorder.TryRecordNamedArgument(attributeData.NamedArguments[i].Key, CommonAttributeParsing.SingleArgument(attributeData.NamedArguments[i].Value), location) is false)
            {
                return false;
            }
        }

        return true;
    }

    private static ExpressionSyntax? TryGetExpressionForNamedArgument(AttributeData attributeData, AttributeSyntax attributeSyntax, int index)
    {
        if (attributeSyntax.ArgumentList!.Arguments.Count == attributeData.ConstructorArguments.Length + attributeData.NamedArguments.Length)
        {
            return attributeSyntax.ArgumentList.Arguments[attributeData.ConstructorArguments.Length + index].Expression;
        }

        var argumentName = attributeData.NamedArguments[index].Key;

        return attributeSyntax.ArgumentList.Arguments.Where((argument) => argument.NameEquals?.Name.Identifier.Text == argumentName).Select((argument) => argument.Expression).FirstOrDefault();
    }

    private static bool CheckIfArgumentMatchesArrayStructure(TypedConstant value, ExpressionSyntax expression)
    {
        if (expression is ArrayCreationExpressionSyntax arrayCreationExpression)
        {
            if (value.Kind is not TypedConstantKind.Array)
            {
                return false;
            }

            if (value.Values.IsDefault)
            {
                return false;
            }

            if (arrayCreationExpression.Initializer is null || arrayCreationExpression.Initializer.Expressions.Count is 0)
            {
                return value.Values.Length is 0;
            }

            if (value.Values.Length != arrayCreationExpression.Initializer.Expressions.Count)
            {
                return false;
            }

            return CheckIfArgumentMatchesArrayStructure(value.Values[0], arrayCreationExpression.Initializer.Expressions[0]);
        }

        if (expression is InitializerExpressionSyntax initializerExpression)
        {
            if (initializerExpression.IsKind(SyntaxKind.ArrayInitializerExpression) is false)
            {
                return value.Kind is not TypedConstantKind.Array;
            }

            if (value.Values.IsDefault)
            {
                return false;
            }

            if (initializerExpression.Expressions.Count is 0)
            {
                return value.Values.Length is 0;
            }

            if (value.Values.Length != initializerExpression.Expressions.Count)
            {
                return false;
            }

            return CheckIfArgumentMatchesArrayStructure(value.Values[0], initializerExpression.Expressions[0]);
        }

        if (expression is ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression)
        {
            return CheckIfArgumentMatchesArrayStructure(value, implicitArrayCreationExpression.Initializer);
        }

        if (expression is CastExpressionSyntax castExpression)
        {
            return CheckIfArgumentMatchesArrayStructure(value, castExpression.Expression);
        }

        if (expression is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            return CheckIfArgumentMatchesArrayStructure(value, parenthesizedExpression.Expression);
        }

        if (expression is LiteralExpressionSyntax && (expression.IsKind(SyntaxKind.NullLiteralExpression) || expression.IsKind(SyntaxKind.DefaultLiteralExpression)))
        {
            if (value.Kind is TypedConstantKind.Array)
            {
                return value.Values.IsDefault;
            }

            return true;
        }

        return value.Kind is not TypedConstantKind.Array;
    }
}
