namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
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

        if (attributeData.AttributeClass is null || attributeData.AttributeClass.TypeKind is TypeKind.Error || attributeData.AttributeConstructor is null)
        {
            return false;
        }

        if (TryParseGenericArguments(recorder, attributeData, attributeSyntax) is false)
        {
            return false;
        }

        if (TryParseConstructorArguments(recorder, attributeData, attributeSyntax) is false)
        {
            return false;
        }

        if (TryParseNamedArguments(recorder, attributeData, attributeSyntax) is false)
        {
            return false;
        }

        return true;
    }

    private bool TryParseGenericArguments(ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (attributeData.AttributeClass is not INamedTypeSymbol attributeType)
        {
            return false;
        }

        if (GetGenericNameSyntax() is not GenericNameSyntax genericNameSyntax)
        {
            return true;
        }

        if (genericNameSyntax.TypeArgumentList.Arguments.Count < attributeType.TypeArguments.Length)
        {
            return false;
        }

        for (var i = 0; i < attributeType.TypeArguments.Length; i++)
        {
            var location = ArgumentLocator.TypeArgument(genericNameSyntax.TypeArgumentList.Arguments[i]);

            if (recorder.TryRecordGenericArgument(attributeType.TypeParameters[i], attributeType.TypeArguments[i], location) is false)
            {
                return false;
            }
        }

        return true;

        GenericNameSyntax? GetGenericNameSyntax()
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

    private bool TryParseConstructorArguments(ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (attributeData.AttributeConstructor is not IMethodSymbol attributeConstructor)
        {
            return false;
        }

        if (attributeData.ConstructorArguments.IsEmpty)
        {
            return true;
        }

        if (attributeSyntax.ArgumentList is null)
        {
            if (attributeConstructor.Parameters.Length is 1 && attributeConstructor.Parameters[0].IsParams)
            {
                var (paramsCollectionLocation, paramsElementLocations) = ArgumentLocator.ParamsArguments(Array.Empty<ExpressionSyntax>());

                return recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[0], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[0]), paramsCollectionLocation, paramsElementLocations);
            }

            return false;
        }

        if (attributeData.ConstructorArguments.Length != attributeConstructor.Parameters.Count())
        {
            return false;
        }

        for (var i = 0; i < attributeData.ConstructorArguments.Length; i++)
        {
            if (attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Error)
            {
                continue;
            }

            if (attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Array)
            {
                if (attributeConstructor.Parameters[i].IsParams)
                {
                    if (TryParseParamsOrArrayArgument(recorder, attributeData, attributeSyntax) is false)
                    {
                        return false;
                    }

                    break;
                }

                var (collectionLocation, elementLocations) = ArgumentLocator.ArrayArgument(attributeSyntax.ArgumentList.Arguments[i].Expression);

                if (recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[i], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[i]), collectionLocation, elementLocations) is false)
                {
                    return false;
                }

                continue;
            }

            var location = ArgumentLocator.SingleArgument(attributeSyntax.ArgumentList.Arguments[i].Expression);

            if (recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[i], CommonAttributeParsing.SingleArgument(attributeData.ConstructorArguments[i]), location) is false)
            {
                return false;
            }
        }

        return true;
    }

    private bool TryParseParamsOrArrayArgument(ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (attributeData.AttributeConstructor is not IMethodSymbol attributeConstructor)
        {
            return false;
        }

        if (attributeSyntax.ArgumentList is null)
        {
            return false;
        }

        var index = attributeData.ConstructorArguments.Length - 1;

        if (attributeData.ConstructorArguments[index].IsNull)
        {
            var (nullCollectionLocation, nullElementLocations) = ArgumentLocator.ArrayArgument(attributeSyntax.ArgumentList.Arguments[index].Expression);

            return recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[index], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[index]), nullCollectionLocation, nullElementLocations);
        }

        if (attributeSyntax.ArgumentList.Arguments.Count < attributeConstructor.Parameters.Length || attributeSyntax.ArgumentList.Arguments[index].NameEquals is not null)
        {
            var (emptyParamsCollectionLocation, emptyParamsElementLocations) = ArgumentLocator.ParamsArguments(Array.Empty<ExpressionSyntax>());

            return recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[index], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[index]), emptyParamsCollectionLocation, emptyParamsElementLocations);
        }

        if (attributeSyntax.ArgumentList.Arguments.Count > attributeConstructor.Parameters.Length && attributeSyntax.ArgumentList.Arguments[index + 1].NameEquals is null)
        {
            var (paramsCollectionLocation, paramsElementLocations) = ArgumentLocator.ParamsArguments(attributeSyntax.ArgumentList.Arguments.Skip(index).Take(attributeSyntax.ArgumentList.Arguments.Count - (attributeData.ConstructorArguments.Length + attributeData.NamedArguments.Length) + 1).Select(static (argument) => argument.Expression).ToArray());

            return recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[index], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[index]), paramsCollectionLocation, paramsElementLocations);
        }

        if (attributeSyntax.ArgumentList.Arguments.Count == attributeConstructor.Parameters.Length || attributeSyntax.ArgumentList.Arguments[index + 1].NameEquals is not null)
        {
            if (attributeData.ConstructorArguments[index].Values.Length is not 1)
            {
                var (arrayCollectionLocation, arrayElementLocations) = ArgumentLocator.ArrayArgument(attributeSyntax.ArgumentList.Arguments[index].Expression);

                return recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[index], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[index]), arrayCollectionLocation, arrayElementLocations);
            }

            var (paramsCollectionLocation, paramsElementLocations) = ArgumentLocator.ParamsArguments(attributeSyntax.ArgumentList.Arguments[index].Expression);

            return recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[index], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[index]), paramsCollectionLocation, paramsElementLocations);
        }

        return false;
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
                var (collectionLocation, elementLocations) = ArgumentLocator.ArrayArgument(argumentExpression);

                if (recorder.TryRecordNamedArgument(attributeData.NamedArguments[i].Key, CommonAttributeParsing.ArrayArguments(attributeData.NamedArguments[i].Value), collectionLocation, elementLocations) is false)
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
}
