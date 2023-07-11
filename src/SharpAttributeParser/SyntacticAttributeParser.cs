namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISyntacticAttributeParser"/>
public sealed class SyntacticAttributeParser : ISyntacticAttributeParser
{
    /// <inheritdoc/>
    public bool TryParse(ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
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

        return TryParseGenericArguments(recorder, attributeType, attributeSyntax) && TryParseConstructorArguments(recorder, attributeData, targetConstructor, attributeSyntax)
            && TryParseNamedArguments(recorder, attributeData, attributeSyntax);
    }

    private bool TryParseGenericArguments(ISyntacticAttributeRecorder recorder, INamedTypeSymbol attributeType, AttributeSyntax attributeSyntax)
    {
        if (getGenericNameSyntax() is not GenericNameSyntax genericNameSyntax)
        {
            return true;
        }

        if (attributeType.TypeParameters.Length != genericNameSyntax.TypeArgumentList.Arguments.Count)
        {
            return false;
        }

        for (var i = 0; i < attributeType.TypeParameters.Length; i++)
        {
            if (attributeType.TypeArguments[i].Kind is SymbolKind.ErrorType)
            {
                return false;
            }

            if (recorder.TryRecordTypeArgumentSyntax(attributeType.TypeParameters[i], genericNameSyntax.TypeArgumentList.Arguments[i]) is false)
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

    private bool TryParseConstructorArguments(ISyntacticAttributeRecorder recorder, AttributeData attributeData, IMethodSymbol targetConstructor, AttributeSyntax attributeSyntax)
    {
        if (attributeData.ConstructorArguments.IsEmpty)
        {
            return true;
        }

        if (attributeSyntax.ArgumentList is null)
        {
            if (targetConstructor.Parameters.Length is 1 && targetConstructor.Parameters[0].IsParams)
            {
                return recorder.TryRecordConstructorParamsArgumentSyntax(targetConstructor.Parameters[0], Array.Empty<ExpressionSyntax>());
            }

            return false;
        }

        for (var i = 0; i < targetConstructor.Parameters.Length; i++)
        {
            if (attributeData.ConstructorArguments[i].Kind is TypedConstantKind.Error)
            {
                return false;
            }

            if (targetConstructor.Parameters[i].IsParams)
            {
                return TryParseParamsOrArrayArgument(recorder, attributeData, targetConstructor, attributeSyntax.ArgumentList.Arguments);
            }

            if (attributeSyntax.ArgumentList.Arguments.Count < i + 1)
            {
                return false;
            }

            if (recorder.TryRecordConstructorArgumentSyntax(targetConstructor.Parameters[i], attributeSyntax.ArgumentList.Arguments[i].Expression) is false)
            {
                return false;
            }
        }

        return true;
    }

    private bool TryParseNamedArguments(ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (attributeSyntax.ArgumentList is null || attributeData.NamedArguments.IsEmpty)
        {
            return true;
        }

        for (var i = 0; i < attributeData.NamedArguments.Length; i++)
        {
            if (attributeData.NamedArguments[i].Value.Kind is TypedConstantKind.Error)
            {
                continue;
            }

            var expression = attributeSyntax.ArgumentList.Arguments[attributeSyntax.ArgumentList.Arguments.Count - attributeData.NamedArguments.Length + i].Expression;

            if (recorder.TryRecordNamedArgumentSyntax(attributeData.NamedArguments[i].Key, expression) is false)
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryParseParamsOrArrayArgument(ISyntacticAttributeRecorder recorder, AttributeData attributeData, IMethodSymbol targetConstructor, IReadOnlyList<AttributeArgumentSyntax> syntacticArguments)
    {
        if (syntacticArguments.Count - attributeData.NamedArguments.Length != targetConstructor.Parameters.Length)
        {
            return TryParseParamsArgument(recorder, attributeData, targetConstructor, syntacticArguments);
        }

        var index = targetConstructor.Parameters.Length - 1;
        var parameter = targetConstructor.Parameters[index];

        if (attributeData.ConstructorArguments[index].IsNull)
        {
            return recorder.TryRecordConstructorArgumentSyntax(parameter, syntacticArguments[index].Expression);
        }

        if (DoesSyntaxMatchArrayStructure(attributeData.ConstructorArguments[index], syntacticArguments[index].Expression))
        {
            return recorder.TryRecordConstructorArgumentSyntax(parameter, syntacticArguments[index].Expression);
        }

        return recorder.TryRecordConstructorParamsArgumentSyntax(parameter, new[] { syntacticArguments[index].Expression });
    }

    private static bool TryParseParamsArgument(ISyntacticAttributeRecorder recorder, AttributeData attributeData, IMethodSymbol targetConstructor, IReadOnlyList<AttributeArgumentSyntax> syntacticArguments)
    {
        return recorder.TryRecordConstructorParamsArgumentSyntax(targetConstructor.Parameters[targetConstructor.Parameters.Length - 1], getElementSyntax());

        IReadOnlyList<ExpressionSyntax> getElementSyntax()
        {
            var paramsArgumentCount = syntacticArguments.Count - attributeData.NamedArguments.Length - targetConstructor.Parameters.Length + 1;

            if (paramsArgumentCount is 0)
            {
                return Array.Empty<ExpressionSyntax>();
            }

            var elementSyntax = new ExpressionSyntax[paramsArgumentCount];

            for (var i = 0; i < elementSyntax.Length; i++)
            {
                elementSyntax[i] = syntacticArguments[targetConstructor.Parameters.Length - 1 + i].Expression;
            }

            return elementSyntax;
        }
    }

    private static bool DoesSyntaxMatchArrayStructure(TypedConstant value, ExpressionSyntax expression) => expression switch
    {
        ArrayCreationExpressionSyntax arrayCreationExpression => DoesSyntaxMatchArrayStructure(value, arrayCreationExpression),
        InitializerExpressionSyntax initializerExpression => DoesSyntaxMatchArrayStructure(value, initializerExpression),
        ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression => DoesSyntaxMatchArrayStructure(value, implicitArrayCreationExpression.Initializer),
        CastExpressionSyntax castExpression => DoesSyntaxMatchArrayStructure(value, castExpression.Expression),
        ParenthesizedExpressionSyntax parenthesizedExpression => DoesSyntaxMatchArrayStructure(value, parenthesizedExpression.Expression),
        LiteralExpressionSyntax literalExpression => DoesSyntaxMatchArrayStructure(value, literalExpression),
        _ => false
    };

    private static bool DoesSyntaxMatchArrayStructure(TypedConstant value, ArrayCreationExpressionSyntax syntax)
    {
        if (value.Kind is not TypedConstantKind.Array)
        {
            return false;
        }

        if (value.Values.IsDefault)
        {
            return false;
        }

        if (syntax.Initializer is null)
        {
            return value.Values.Length is 0;
        }

        return DoesSyntaxMatchArrayStructure(value, syntax.Initializer);
    }

    private static bool DoesSyntaxMatchArrayStructure(TypedConstant value, InitializerExpressionSyntax syntax)
    {
        if (syntax.IsKind(SyntaxKind.ArrayInitializerExpression) is false)
        {
            return value.Kind is not TypedConstantKind.Array;
        }

        if (value.Values.IsDefault)
        {
            return false;
        }

        if (syntax.Expressions.Count is 0)
        {
            return value.Values.Length is 0;
        }

        if (value.Values.Length != syntax.Expressions.Count)
        {
            return false;
        }

        return DoesSyntaxMatchArrayStructure(value.Values[0], syntax.Expressions[0]);
    }

    private static bool DoesSyntaxMatchArrayStructure(TypedConstant value, LiteralExpressionSyntax syntax)
    {
        if (syntax.IsKind(SyntaxKind.NullLiteralExpression) || syntax.IsKind(SyntaxKind.DefaultLiteralExpression))
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
