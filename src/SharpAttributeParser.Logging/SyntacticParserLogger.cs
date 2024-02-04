namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging.SyntacticParserLoggerComponents;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISyntacticParserLogger{TCategoryName}"/>
public sealed class SyntacticParserLogger<TCategoryName> : ISyntacticParserLogger<TCategoryName>
{
    private readonly ITypeArgumentLogger TypeArgument;
    private readonly IConstructorArgumentLogger ConstructorArgument;
    private readonly INamedArgumentLogger NamedArgument;

    private readonly ILogger Logger;

    /// <summary>Instantiates a <see cref="SyntacticParserLogger{TCategoryName}"/>, handling logging for <see cref="ISyntacticParser"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public SyntacticParserLogger(ILogger<TCategoryName> logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        TypeArgument = new TypeArgumentLogger(Logger);
        ConstructorArgument = new ConstructorArgumentLogger(Logger);
        NamedArgument = new NamedArgumentLogger(Logger);
    }

    ITypeArgumentLogger ISyntacticParserLogger.Type => TypeArgument;
    IConstructorArgumentLogger ISyntacticParserLogger.Constructor => ConstructorArgument;
    INamedArgumentLogger ISyntacticParserLogger.Named => NamedArgument;

    IDisposable? ISyntacticParserLogger.BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (recorderType is null)
        {
            throw new ArgumentNullException(nameof(recorderType));
        }

        return ScopeDefinitions.ParsingAttribute(Logger, recorderType);
    }

    IDisposable? ISyntacticParserLogger.BeginScopeDeterminedAttributeClass(INamedTypeSymbol attributeClass)
    {
        if (attributeClass is null)
        {
            throw new ArgumentNullException(nameof(attributeClass));
        }

        return ScopeDefinitions.DeterminedAttributeClass(Logger, attributeClass.Name);
    }

    IDisposable? ISyntacticParserLogger.BeginScopeDeterminedTargetConstructor(IMethodSymbol targetConstructor) => null;

    void ISyntacticParserLogger.UndeterminedAttributeClass() => MessageDefinitions.UndeterminedAttributeClass(Logger, null);
    void ISyntacticParserLogger.UnrecognizedAttributeClass() => MessageDefinitions.UnrecognizedAttributeClass(Logger, null);
    void ISyntacticParserLogger.UndeterminedTargetConstructor() => MessageDefinitions.UndeterminedTargetConstructor(Logger, null);

    private sealed class TypeArgumentLogger : ITypeArgumentLogger
    {
        private readonly ILogger Logger;

        public TypeArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? ITypeArgumentLogger.BeginScopeParsingTypeArguments(IReadOnlyList<ITypeParameterSymbol> parameters, NameSyntax nameSyntax)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (nameSyntax is null)
            {
                throw new ArgumentNullException(nameof(nameSyntax));
            }

            return ScopeDefinitions.ParsingTypeArguments(Logger, parameters.Count);
        }

        IDisposable? ITypeArgumentLogger.BeginScopeParsingTypeArgument(ITypeParameterSymbol parameter, TypeSyntax syntax)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return ScopeDefinitions.ParsingTypeArgument(Logger, parameter.Name, parameter.Ordinal);
        }

        IDisposable? ITypeArgumentLogger.BeginScopeRecognizedGenericExpression(GenericNameSyntax genericNameSyntax)
        {
            if (genericNameSyntax is null)
            {
                throw new ArgumentNullException(nameof(genericNameSyntax));
            }

            return ScopeDefinitions.ParsingTypeArgumentsOfGenericType(Logger, genericNameSyntax.Arity);
        }

        void ITypeArgumentLogger.SyntaxNotRecognizedAsGenericExpression() => MessageDefinitions.SyntaxNotRecognizedAsGenericExpression(Logger, null);
        void ITypeArgumentLogger.UnexpectedNumberOfSyntacticTypeArguments() => MessageDefinitions.UnexpectedNumberOfSyntacticTypeArguments(Logger, null);
    }

    private sealed class ConstructorArgumentLogger : IConstructorArgumentLogger
    {
        private readonly ILogger Logger;

        public ConstructorArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? IConstructorArgumentLogger.BeginScopeParsingConstructorArguments(AttributeData attributeData, AttributeSyntax attributeSyntax)
        {
            if (attributeData is null)
            {
                throw new ArgumentNullException(nameof(attributeData));
            }

            return ScopeDefinitions.ParsingConstructorArguments(Logger, attributeData.ConstructorArguments.Length);
        }

        IDisposable? IConstructorArgumentLogger.BeginScopeParsingConstructorArgument(IParameterSymbol parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return ScopeDefinitions.ParsingConstructorArgument(Logger, parameter.Name, parameter.Ordinal);
        }

        IDisposable? IConstructorArgumentLogger.BeginScopeParsingNormalConstructorArgument(ExpressionSyntax syntax) => ScopeDefinitions.ParsingConstructorArgumentOfKind(Logger, "Normal");
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingParamsConstructorArgument(IReadOnlyList<ExpressionSyntax> elementSyntax) => ScopeDefinitions.ParsingConstructorArgumentOfKind(Logger, "Params");
        IDisposable? IConstructorArgumentLogger.BeginScopeParsingDefaultConstructorArgument() => ScopeDefinitions.ParsingConstructorArgumentOfKind(Logger, "Default");

        void IConstructorArgumentLogger.MissingOneOrMoreRequiredConstructorArgument() => MessageDefinitions.MissingOneOrMoreRequiredConstructorArgument(Logger, null);
        void IConstructorArgumentLogger.OutOfOrderLabelledConstructorArgumentsFollowedByUnlabelled() => MessageDefinitions.OutOfOrderLabelledConstructorArgumentFollowedByUnlabelled(Logger, null);
        void IConstructorArgumentLogger.UnexpectedNumberOfSemanticConstructorArguments() => MessageDefinitions.UnexpectedNumberOfSemanticConstructorArguments(Logger, null);
        void IConstructorArgumentLogger.LabelledConstructorArgumentHadNoMatchingParameter() => MessageDefinitions.LabelledConstructorArgumentHadNoMatchingParameter(Logger, null);
    }

    private sealed class NamedArgumentLogger : INamedArgumentLogger
    {
        private readonly ILogger Logger;

        public NamedArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? INamedArgumentLogger.BeginScopeParsingNamedArguments(AttributeData attributeData, AttributeSyntax attributeSyntax)
        {
            if (attributeData is null)
            {
                throw new ArgumentNullException(nameof(attributeData));
            }

            return ScopeDefinitions.ParsingNamedArguments(Logger, attributeData.NamedArguments.Length);
        }

        IDisposable? INamedArgumentLogger.BeginScopeParsingNamedArgument(string parameterName, ExpressionSyntax syntax)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return ScopeDefinitions.ParsingNamedArgument(Logger, parameterName);
        }
    }

    private static class ScopeDefinitions
    {
        public static Func<ILogger, Type, IDisposable?> ParsingAttribute { get; }
        public static Func<ILogger, string, IDisposable?> DeterminedAttributeClass { get; }

        public static Func<ILogger, int, IDisposable?> ParsingTypeArguments { get; }
        public static Func<ILogger, string, int, IDisposable?> ParsingTypeArgument { get; }
        public static Func<ILogger, int, IDisposable?> ParsingTypeArgumentsOfGenericType { get; }

        public static Func<ILogger, int, IDisposable?> ParsingConstructorArguments { get; }
        public static Func<ILogger, string, int, IDisposable?> ParsingConstructorArgument { get; }
        public static Func<ILogger, string, IDisposable?> ParsingConstructorArgumentOfKind { get; }

        public static Func<ILogger, int, IDisposable?> ParsingNamedArguments { get; }
        public static Func<ILogger, string, IDisposable?> ParsingNamedArgument { get; }

        static ScopeDefinitions()
        {
            ParsingAttribute = LoggerMessage.DefineScope<Type>("RecorderType: {RecorderType}");
            DeterminedAttributeClass = LoggerMessage.DefineScope<string>("AttributeClassName: {AttributeClassName}");

            ParsingTypeArguments = LoggerMessage.DefineScope<int>("TypeParameterCount: {TypeParameterCount}");
            ParsingTypeArgument = LoggerMessage.DefineScope<string, int>("[TypeParameterName: {TypeParameterName}, TypeParameterIndex: {TypeParameterIndex}");
            ParsingTypeArgumentsOfGenericType = LoggerMessage.DefineScope<int>("SyntacticTypeArgumentCount: {SyntacticTypeArgumentCount}");

            ParsingConstructorArguments = LoggerMessage.DefineScope<int>("SemanticConstructorArgumentCount: {SemanticConstructorArgumentCount}");
            ParsingConstructorArgument = LoggerMessage.DefineScope<string, int>("[ConstructorParameterName: {ConstructorParameterName}, ConstructorParameterIndex: {ConstructorParameterIndex}]");
            ParsingConstructorArgumentOfKind = LoggerMessage.DefineScope<string>("ConstructorArgumentKind: {ConstructorArgumentKind}");

            ParsingNamedArguments = LoggerMessage.DefineScope<int>("NamedArgumentCount: {NamedArgumentCount}");
            ParsingNamedArgument = LoggerMessage.DefineScope<string>("NamedParameterName: {NamedParameterName}");
        }
    }

    private static class MessageDefinitions
    {
        public static Action<ILogger, Exception?> UndeterminedAttributeClass { get; }
        public static Action<ILogger, Exception?> UnrecognizedAttributeClass { get; }
        public static Action<ILogger, Exception?> UndeterminedTargetConstructor { get; }

        public static Action<ILogger, Exception?> SyntaxNotRecognizedAsGenericExpression { get; }
        public static Action<ILogger, Exception?> UnexpectedNumberOfSyntacticTypeArguments { get; }

        public static Action<ILogger, Exception?> MissingOneOrMoreRequiredConstructorArgument { get; }
        public static Action<ILogger, Exception?> OutOfOrderLabelledConstructorArgumentFollowedByUnlabelled { get; }
        public static Action<ILogger, Exception?> UnexpectedNumberOfSemanticConstructorArguments { get; }
        public static Action<ILogger, Exception?> LabelledConstructorArgumentHadNoMatchingParameter { get; }

        static MessageDefinitions()
        {
            UndeterminedAttributeClass = LoggerMessage.Define(LogLevel.Debug, EventIDs.UndeterminedAttributeClass, "Failed to syntactically parse an attribute, as the class of the attribute could not be determined.");
            UnrecognizedAttributeClass = LoggerMessage.Define(LogLevel.Debug, EventIDs.UnrecognizedAttributeClass, "Failed to syntactically parse an attribute, as the class of the attribute was not recognized.");
            UndeterminedTargetConstructor = LoggerMessage.Define(LogLevel.Debug, EventIDs.UndeterminedTargetConstructor, "Failed to syntactically parse an attribute, as the target constructor could not be determined.");

            SyntaxNotRecognizedAsGenericExpression = LoggerMessage.Define(LogLevel.Debug, EventIDs.SyntaxNotRecognizedAsGenericExpression, "Failed to syntactically parse the type arguments of an attribute, as the attribute was generic but the syntax was not recognized as expressing a generic type.");
            UnexpectedNumberOfSyntacticTypeArguments = LoggerMessage.Define(LogLevel.Debug, EventIDs.UnexpectedNumberOfSyntacticArguments, "Failed to syntactically parse the type arguments of an attribute, as the number of syntactic arguments differed from the arity of the attribute.");

            MissingOneOrMoreRequiredConstructorArgument = LoggerMessage.Define(LogLevel.Debug, EventIDs.MissingOneOrMoreRequiredConstructorArgument, "Failed to syntactically parse the constructor arguments of an attribute, as one or more required argument was not provided.");
            OutOfOrderLabelledConstructorArgumentFollowedByUnlabelled = LoggerMessage.Define(LogLevel.Debug, EventIDs.OutOfOrderLabelledConstructorArgumentFollowedByUnlabelled, "Failed to syntactically parse the constructor arguments of an attribute, as an out-of-order argument was followed by an unlabelled argument.");
            UnexpectedNumberOfSemanticConstructorArguments = LoggerMessage.Define(LogLevel.Debug, EventIDs.UnexpectedNumberOfSemanticConstructorArguments, "Failed to syntactically parse the constructor arguments of an attribute, as the number of arguments differed from the arity of the target constructor.");
            LabelledConstructorArgumentHadNoMatchingParameter = LoggerMessage.Define(LogLevel.Debug, EventIDs.LabelledConstructorArgumentHadNoMatchingParameter, "Failed to syntactically parse a labelled constructor argument, as the label did not match any parameter.");
        }
    }

    private static class EventIDs
    {
        public static EventId UndeterminedAttributeClass { get; }
        public static EventId UnrecognizedAttributeClass { get; }
        public static EventId UndeterminedTargetConstructor { get; }

        public static EventId SyntaxNotRecognizedAsGenericExpression { get; }
        public static EventId UnexpectedNumberOfSyntacticArguments { get; }

        public static EventId MissingOneOrMoreRequiredConstructorArgument { get; }
        public static EventId OutOfOrderLabelledConstructorArgumentFollowedByUnlabelled { get; }
        public static EventId UnexpectedNumberOfSemanticConstructorArguments { get; }
        public static EventId LabelledConstructorArgumentHadNoMatchingParameter { get; }

        static EventIDs()
        {
            SequentialEventID eventIDs = new();

            UndeterminedAttributeClass = new(eventIDs.Next, nameof(UndeterminedAttributeClass));
            UnrecognizedAttributeClass = new(eventIDs.Next, nameof(UnrecognizedAttributeClass));
            UndeterminedTargetConstructor = new(eventIDs.Next, nameof(UndeterminedTargetConstructor));

            SyntaxNotRecognizedAsGenericExpression = new(eventIDs.Next, nameof(SyntaxNotRecognizedAsGenericExpression));
            UnexpectedNumberOfSyntacticArguments = new(eventIDs.Next, nameof(UnexpectedNumberOfSyntacticArguments));

            MissingOneOrMoreRequiredConstructorArgument = new(eventIDs.Next, nameof(MissingOneOrMoreRequiredConstructorArgument));
            OutOfOrderLabelledConstructorArgumentFollowedByUnlabelled = new(eventIDs.Next, nameof(OutOfOrderLabelledConstructorArgumentFollowedByUnlabelled));
            UnexpectedNumberOfSemanticConstructorArguments = new(eventIDs.Next, nameof(UnexpectedNumberOfSemanticConstructorArguments));
            LabelledConstructorArgumentHadNoMatchingParameter = new(eventIDs.Next, nameof(LabelledConstructorArgumentHadNoMatchingParameter));
        }
    }
}

