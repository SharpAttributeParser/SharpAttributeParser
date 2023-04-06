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

        if (attributeSyntax.Name is not GenericNameSyntax genericNameSyntax)
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
            return false;
        }

        if (attributeData.ConstructorArguments.Length != attributeConstructor.Parameters.Count() || attributeSyntax.ArgumentList.Arguments.Count < attributeData.ConstructorArguments.Length)
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
                if (i == attributeConstructor.Parameters.Count() - 1 && attributeSyntax.ArgumentList.Arguments.Count > attributeData.ConstructorArguments.Length + attributeData.NamedArguments.Length)
                {
                    if (TryParseParamsArgument(recorder, attributeData, attributeSyntax) is false)
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

    private bool TryParseParamsArgument(ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
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

        var (paramsCollectionLocation, paramsElementLocations) = ArgumentLocator.ParamsArguments(attributeSyntax.ArgumentList.Arguments.Skip(index).Take(attributeSyntax.ArgumentList.Arguments.Count - (attributeData.ConstructorArguments.Length + attributeData.NamedArguments.Length) + 1).Select(static (argument) => argument.Expression).ToArray());

        return recorder.TryRecordConstructorArgument(attributeConstructor.Parameters[index], CommonAttributeParsing.ArrayArguments(attributeData.ConstructorArguments[index]), paramsCollectionLocation, paramsElementLocations);
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

        if (attributeSyntax.ArgumentList.Arguments.Count < attributeData.ConstructorArguments.Length + attributeData.NamedArguments.Length)
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
