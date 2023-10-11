namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging.SemanticParserComponents;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISemanticParserLogger{TCategoryName}"/>
public sealed class SemanticParserLogger<TCategoryName> : ISemanticParserLogger<TCategoryName>
{
    private readonly ITypeArgumentLogger TypeArgument;
    private readonly IConstructorArgumentLogger ConstructorArgument;
    private readonly INamedArgumentLogger NamedArgument;

    private readonly ILogger Logger;

    /// <summary>Instantiates a <see cref="SemanticParserLogger{TCategoryName}"/>, handling logging for <see cref="ISemanticParser"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    public SemanticParserLogger(ILogger<TCategoryName> logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        TypeArgument = new TypeArgumentLogger(Logger);
        ConstructorArgument = new ConstructorArgumentLogger(Logger);
        NamedArgument = new NamedArgumentLogger(Logger);
    }

    ITypeArgumentLogger ISemanticParserLogger.TypeArgument => TypeArgument;
    IConstructorArgumentLogger ISemanticParserLogger.ConstructorArgument => ConstructorArgument;
    INamedArgumentLogger ISemanticParserLogger.NamedArgument => NamedArgument;

    IDisposable? ISemanticParserLogger.BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData)
    {
        if (recorderType is null)
        {
            throw new ArgumentNullException(nameof(recorderType));
        }

        return ScopeDefinitions.ParsingAttribute(Logger, recorderType);
    }

    IDisposable? ISemanticParserLogger.BeginScopeDeterminedAttributeClass(INamedTypeSymbol attributeClass)
    {
        if (attributeClass is null)
        {
            throw new ArgumentNullException(nameof(attributeClass));
        }

        return ScopeDefinitions.DeterminedAttributeClass(Logger, attributeClass.Name);
    }

    IDisposable? ISemanticParserLogger.BeginScopeDeterminedTargetConstructor(IMethodSymbol targetConstructor) => null;

    void ISemanticParserLogger.UndeterminedAttributeClass() => MessageDefinitions.UndeterminedAttributeClass(Logger, null);
    void ISemanticParserLogger.UnrecognizedAttributeClass() => MessageDefinitions.UnrecognizedAttributeClass(Logger, null);
    void ISemanticParserLogger.UndeterminedTargetConstructor() => MessageDefinitions.UndeterminedTargetConstructor(Logger, null);

    private sealed class TypeArgumentLogger : ITypeArgumentLogger
    {
        private readonly ILogger Logger;

        public TypeArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? ITypeArgumentLogger.BeginScopeParsingTypeArguments(IReadOnlyList<ITypeParameterSymbol> parameters, IReadOnlyList<ITypeSymbol> arguments)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (arguments is null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            return ScopeDefinitions.ParsingTypeArguments(Logger, parameters.Count, arguments.Count);
        }

        IDisposable? ITypeArgumentLogger.BeginScopeParsingTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return ScopeDefinitions.ParsingTypeArgument(Logger, parameter.Name, parameter.Ordinal, argument.Name);
        }

        void ITypeArgumentLogger.UnexpectedNumberOfTypeArguments() => MessageDefinitions.UnexpectedNumberOfTypeArguments(Logger, null);
        void ITypeArgumentLogger.InvalidTypeArgument() => MessageDefinitions.InvalidTypeArgument(Logger, null);
    }

    private sealed class ConstructorArgumentLogger : IConstructorArgumentLogger
    {
        private readonly ILogger Logger;

        public ConstructorArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? IConstructorArgumentLogger.BeginScopeParsingConstructorArguments(IReadOnlyList<IParameterSymbol> parameters, IReadOnlyList<TypedConstant> arguments)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (arguments is null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            return ScopeDefinitions.ParsingConstructorArguments(Logger, parameters.Count, arguments.Count);
        }

        IDisposable? IConstructorArgumentLogger.BeginScopeParsingConstructorArgument(IParameterSymbol parameter, TypedConstant argument)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return ScopeDefinitions.ParsingConstructorArgument(Logger, parameter.Name, parameter.Ordinal);
        }

        void IConstructorArgumentLogger.UnexpectedNumberOfConstructorArguments() => MessageDefinitions.UnexpectedNumberOfConstructorArguments(Logger, null);
        void IConstructorArgumentLogger.InvalidConstructorArgument() => MessageDefinitions.InvalidConstructorArgument(Logger, null);
    }

    private sealed class NamedArgumentLogger : INamedArgumentLogger
    {
        private readonly ILogger Logger;

        public NamedArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? INamedArgumentLogger.BeginScopeParsingNamedArguments(IReadOnlyList<KeyValuePair<string, TypedConstant>> arguments)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            return ScopeDefinitions.ParsingNamedArguments(Logger, arguments.Count);
        }

        IDisposable? INamedArgumentLogger.BeginScopeParsingNamedArgument(string parameterName, TypedConstant argument)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return ScopeDefinitions.ParsingNamedArgument(Logger, parameterName);
        }

        void INamedArgumentLogger.InvalidNamedArgument() => MessageDefinitions.InvalidNamedArgument(Logger, null);
    }

    private static class ScopeDefinitions
    {
        public static Func<ILogger, Type, IDisposable?> ParsingAttribute { get; }
        public static Func<ILogger, string, IDisposable?> DeterminedAttributeClass { get; }

        public static Func<ILogger, int, int, IDisposable?> ParsingTypeArguments { get; }
        public static Func<ILogger, string, int, string, IDisposable?> ParsingTypeArgument { get; }

        public static Func<ILogger, int, int, IDisposable?> ParsingConstructorArguments { get; }
        public static Func<ILogger, string, int, IDisposable?> ParsingConstructorArgument { get; }

        public static Func<ILogger, int, IDisposable?> ParsingNamedArguments { get; }
        public static Func<ILogger, string, IDisposable?> ParsingNamedArgument { get; }

        static ScopeDefinitions()
        {
            ParsingAttribute = LoggerMessage.DefineScope<Type>("RecorderType: {RecorderType}");
            DeterminedAttributeClass = LoggerMessage.DefineScope<string>("AttributeClassName: {AttributeClassName}");

            ParsingTypeArguments = LoggerMessage.DefineScope<int, int>("[TypeParameterCount: {TypeParameterCount}, TypeArgumentCount: {TypeArgumentCount}]");
            ParsingTypeArgument = LoggerMessage.DefineScope<string, int, string>("[TypeParameterName: {TypeParameterName}, TypeParameterIndex: {TypeParameterIndex}, TypeArgumentClassName: {TypeArgumentClassName}]");

            ParsingConstructorArguments = LoggerMessage.DefineScope<int, int>("[ConstructorParameterCount: {ConstructorParameterCount}, ConstructorArgumentCount: {ConstructorArgumentCount}]");
            ParsingConstructorArgument = LoggerMessage.DefineScope<string, int>("[ConstructorParameterName: {ConstructorParameterName}, ConstructorParameterIndex: {ConstructorParameterIndex}]");

            ParsingNamedArguments = LoggerMessage.DefineScope<int>("NamedArgumentCount: {NamedArgumentCount}");
            ParsingNamedArgument = LoggerMessage.DefineScope<string>("NamedParameterName: {NamedParameterName}");
        }
    }

    private static class MessageDefinitions
    {
        public static Action<ILogger, Exception?> UndeterminedAttributeClass { get; }
        public static Action<ILogger, Exception?> UnrecognizedAttributeClass { get; }
        public static Action<ILogger, Exception?> UndeterminedTargetConstructor { get; }

        public static Action<ILogger, Exception?> UnexpectedNumberOfTypeArguments { get; }
        public static Action<ILogger, Exception?> InvalidTypeArgument { get; }

        public static Action<ILogger, Exception?> UnexpectedNumberOfConstructorArguments { get; }
        public static Action<ILogger, Exception?> InvalidConstructorArgument { get; }

        public static Action<ILogger, Exception?> InvalidNamedArgument { get; }

        static MessageDefinitions()
        {
            UndeterminedAttributeClass = LoggerMessage.Define(LogLevel.Debug, EventIDs.UndeterminedAttributeClass, "Failed to parse an attribute, as the class of the attribute could not be determined.");
            UnrecognizedAttributeClass = LoggerMessage.Define(LogLevel.Debug, EventIDs.UnrecognizedAttributeClass, "Failed to parse an attribute, as the class of the attribute was not recognized.");
            UndeterminedTargetConstructor = LoggerMessage.Define(LogLevel.Debug, EventIDs.UndeterminedTargetConstructor, "Failed to parse an attribute, as the target constructor could not be determined.");

            UnexpectedNumberOfTypeArguments = LoggerMessage.Define(LogLevel.Debug, EventIDs.UnexpectedNumberOfTypeArguments, "Failed to parse the type arguments of an attribute, as the number of syntactic arguments differed from the arity of the attribute.");
            InvalidTypeArgument = LoggerMessage.Define(LogLevel.Debug, EventIDs.InvalidTypeArgument, "Failed to parse a type argument, as the argument was invalid.");

            UnexpectedNumberOfConstructorArguments = LoggerMessage.Define(LogLevel.Debug, EventIDs.UnexpectedNumberOfConstructorArguments, "Failed to parse the constructor arguments of an attribute, as the number of arguments differed from the arity of the target constructor.");
            InvalidConstructorArgument = LoggerMessage.Define(LogLevel.Debug, EventIDs.InvalidConstructorArgument, "Failed to parse a constructor argument, as the argument was invalid.");

            InvalidNamedArgument = LoggerMessage.Define(LogLevel.Debug, EventIDs.InvalidNamedArgument, "Failed to parse a named argument, as the argument was invalid.");
        }
    }

    private static class EventIDs
    {
        public static EventId UndeterminedAttributeClass { get; }
        public static EventId UnrecognizedAttributeClass { get; }
        public static EventId UndeterminedTargetConstructor { get; }

        public static EventId UnexpectedNumberOfTypeArguments { get; }
        public static EventId InvalidTypeArgument { get; }

        public static EventId UnexpectedNumberOfConstructorArguments { get; }
        public static EventId InvalidConstructorArgument { get; }

        public static EventId InvalidNamedArgument { get; }

        static EventIDs()
        {
            SequentialEventID eventIDs = new();

            UndeterminedAttributeClass = new(eventIDs.Next, nameof(UndeterminedAttributeClass));
            UnrecognizedAttributeClass = new(eventIDs.Next, nameof(UnrecognizedAttributeClass));
            UndeterminedTargetConstructor = new(eventIDs.Next, nameof(UndeterminedTargetConstructor));

            UnexpectedNumberOfTypeArguments = new(eventIDs.Next, nameof(UnexpectedNumberOfTypeArguments));
            InvalidTypeArgument = new(eventIDs.Next, nameof(InvalidTypeArgument));

            UnexpectedNumberOfConstructorArguments = new(eventIDs.Next, nameof(UnexpectedNumberOfConstructorArguments));
            InvalidConstructorArgument = new(eventIDs.Next, nameof(InvalidConstructorArgument));

            InvalidNamedArgument = new(eventIDs.Next, nameof(InvalidNamedArgument));
        }
    }
}

