namespace SharpAttributeParser.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

using SharpAttributeParser.Logging.CombinedParserComponents;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ICombinedParserLogger{TCategoryName}"/>
public sealed class CombinedParserLogger<TCategoryName> : ICombinedParserLogger<TCategoryName>
{
    private ITypeArgumentLogger TypeArgument { get; }
    private IConstructorArgumentLogger ConstructorArgument { get; }
    private INamedArgumentLogger NamedArgument { get; }

    private ILogger Logger { get; }

    /// <summary>Instantiates a <see cref="CombinedParserLogger{TCategoryName}"/>, handling logging for <see cref="ICombinedParser"/>.</summary>
    /// <param name="logger">The logger used to log messages.</param>
    /// <exception cref="ArgumentNullException"/>
    public CombinedParserLogger(ILogger<TCategoryName> logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        TypeArgument = new TypeArgumentLogger(Logger);
        ConstructorArgument = new ConstructorArgumentLogger(Logger);
        NamedArgument = new NamedArgumentLogger(Logger);
    }

    ITypeArgumentLogger ICombinedParserLogger.TypeArgument => TypeArgument;
    IConstructorArgumentLogger ICombinedParserLogger.ConstructorArgument => ConstructorArgument;
    INamedArgumentLogger ICombinedParserLogger.NamedArgument => NamedArgument;

    IDisposable? ICombinedParserLogger.BeginScopeParsingAttribute(Type recorderType, AttributeData attributeData, AttributeSyntax attributeSyntax)
    {
        if (recorderType is null)
        {
            throw new ArgumentNullException(nameof(recorderType));
        }

        return ScopeDefinitions.ParsingAttribute(Logger, recorderType);
    }

    void ICombinedParserLogger.SemanticParserFailedToParseAttribute() => MessageDefinitions.SemanticParserFailedToParseAttribute(Logger, null);
    void ICombinedParserLogger.SyntacticParserFailedToParseAttribute() => MessageDefinitions.SyntacticParserFailedToParseAttribute(Logger, null);

    private sealed class TypeArgumentLogger : ITypeArgumentLogger
    {
        private ILogger Logger { get; }

        public TypeArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? ITypeArgumentLogger.BeginScopePropagateParsedTypeArguments(IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> semanticArguments, IReadOnlyDictionary<ITypeParameterSymbol, ExpressionSyntax> syntacticArguments)
        {
            if (semanticArguments is null)
            {
                throw new ArgumentNullException(nameof(semanticArguments));
            }

            if (syntacticArguments is null)
            {
                throw new ArgumentNullException(nameof(syntacticArguments));
            }

            return ScopeDefinitions.PropagateParsedTypeArguments(Logger, semanticArguments.Count, syntacticArguments.Count);
        }

        IDisposable? ITypeArgumentLogger.BeginScopePropagateSemanticallyParsedTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return ScopeDefinitions.PropagateSemanticallyParsedTypeArgument(Logger, parameter.Name, parameter.Ordinal, argument.Name);
        }

        IDisposable? ITypeArgumentLogger.BeginScopePropagateSyntacticallyParsedTypeArgument(ExpressionSyntax syntax) => null;

        void ITypeArgumentLogger.DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments() => MessageDefinitions.DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments(Logger, null);
        void ITypeArgumentLogger.TypeArgumentOnlyRecordedSemantically() => MessageDefinitions.TypeArgumentOnlyRecordedSemantically(Logger, null);
    }

    private sealed class ConstructorArgumentLogger : IConstructorArgumentLogger
    {
        private ILogger Logger { get; }

        public ConstructorArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? IConstructorArgumentLogger.BeginScopePropagateParsedConstructorArguments(IReadOnlyDictionary<IParameterSymbol, object?> semanticArguments, IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> nonParamsSyntacticArguments, IReadOnlyDictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> paramsSyntacticArguments, ISet<IParameterSymbol> defaultSyntacticArguments)
        {
            if (semanticArguments is null)
            {
                throw new ArgumentNullException(nameof(semanticArguments));
            }

            if (nonParamsSyntacticArguments is null)
            {
                throw new ArgumentNullException(nameof(nonParamsSyntacticArguments));
            }

            if (paramsSyntacticArguments is null)
            {
                throw new ArgumentNullException(nameof(paramsSyntacticArguments));
            }

            if (defaultSyntacticArguments is null)
            {
                throw new ArgumentNullException(nameof(defaultSyntacticArguments));
            }

            return ScopeDefinitions.PropagateParsedConstructorArguments(Logger, semanticArguments.Count, nonParamsSyntacticArguments.Count + paramsSyntacticArguments.Count + defaultSyntacticArguments.Count);
        }

        IDisposable? IConstructorArgumentLogger.BeginScopePropagateSemanticallyParsedConstructorArgument(IParameterSymbol parameter, object? argument) => BeginScopePropagateParsedConstructorArgument(parameter);

        private IDisposable? BeginScopePropagateParsedConstructorArgument(IParameterSymbol parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return ScopeDefinitions.PropagateParsedConstructorArgument(Logger, parameter.Name, parameter.Ordinal);
        }

        IDisposable? IConstructorArgumentLogger.BeginScopePropagateSyntacticallyParsedNormalConstructorArgument(ExpressionSyntax syntax) => ScopeDefinitions.PropagateParsedConstructorArgumentOfKind(Logger, "Normal");
        IDisposable? IConstructorArgumentLogger.BeginScopePropagateSyntacticallyParsedParamsConstructorArgument(IReadOnlyList<ExpressionSyntax> elementSyntax) => ScopeDefinitions.PropagateParsedConstructorArgumentOfKind(Logger, "Params");
        IDisposable? IConstructorArgumentLogger.BeginScopePropagateSyntacticallyParsedDefaultConstructorArgument() => ScopeDefinitions.PropagateParsedConstructorArgumentOfKind(Logger, "Default");

        void IConstructorArgumentLogger.DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments() => MessageDefinitions.DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments(Logger, null);
        void IConstructorArgumentLogger.ConstructorArgumentOnlyRecordedSemantically() => MessageDefinitions.ConstructorArgumentOnlyRecordedSemantically(Logger, null);
    }

    private sealed class NamedArgumentLogger : INamedArgumentLogger
    {
        private ILogger Logger { get; }

        public NamedArgumentLogger(ILogger logger)
        {
            Logger = logger;
        }

        IDisposable? INamedArgumentLogger.BeginScopePropagateParsedNamedArguments(IReadOnlyDictionary<string, object?> semanticArguments, IReadOnlyDictionary<string, ExpressionSyntax> syntacticArguments)
        {
            if (semanticArguments is null)
            {
                throw new ArgumentNullException(nameof(semanticArguments));
            }

            if (syntacticArguments is null)
            {
                throw new ArgumentNullException(nameof(syntacticArguments));
            }

            return ScopeDefinitions.PropagateParsedNamedArguments(Logger, semanticArguments.Count, syntacticArguments.Count);
        }

        IDisposable? INamedArgumentLogger.BeginScopePropagateSemanticallyParsedNamedArgument(string parameterName, object? argument) => BeginScopePropagateParsedNamedArgument(parameterName);

        private IDisposable? BeginScopePropagateParsedNamedArgument(string parameterName)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            return ScopeDefinitions.PropagateParsedNamedArgument(Logger, parameterName);
        }

        IDisposable? INamedArgumentLogger.BeginScopePropagateSyntacticallyParsedNamedArgument(ExpressionSyntax syntax) => null;

        void INamedArgumentLogger.DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments() => MessageDefinitions.DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments(Logger, null);
        void INamedArgumentLogger.NamedArgumentOnlyRecordedSemantically() => MessageDefinitions.NamedArgumentOnlyRecordedSemantically(Logger, null);
    }

    private static class ScopeDefinitions
    {
        public static Func<ILogger, Type, IDisposable?> ParsingAttribute { get; }

        public static Func<ILogger, int, int, IDisposable?> PropagateParsedTypeArguments { get; }
        public static Func<ILogger, string, int, string, IDisposable?> PropagateSemanticallyParsedTypeArgument { get; }

        public static Func<ILogger, int, int, IDisposable?> PropagateParsedConstructorArguments { get; }
        public static Func<ILogger, string, int, IDisposable?> PropagateParsedConstructorArgument { get; }
        public static Func<ILogger, string, IDisposable?> PropagateParsedConstructorArgumentOfKind { get; }

        public static Func<ILogger, int, int, IDisposable?> PropagateParsedNamedArguments { get; }
        public static Func<ILogger, string, IDisposable?> PropagateParsedNamedArgument { get; }

        static ScopeDefinitions()
        {
            ParsingAttribute = LoggerMessage.DefineScope<Type>("RecorderType: {RecorderType}");

            PropagateParsedTypeArguments = LoggerMessage.DefineScope<int, int>("[SemanticallyParsedTypeArgumentCount: {SemanticallyParsedTypeArgumentCount}, SyntacticallyParsedTypeArgumentCount: {SyntacticallyParsedTypeArgumentCount}]");
            PropagateSemanticallyParsedTypeArgument = LoggerMessage.DefineScope<string, int, string>("[TypeParameterName: {TypeParameterName}, TypeParameterIndex: {TypeParameterIndex}, TypeArgumentClassName: {TypeArgumentClassName}]");

            PropagateParsedConstructorArguments = LoggerMessage.DefineScope<int, int>("[SemanticallyParsedConstructorArgumentCount: {SemanticallyParsedConstructorArgumentCount}, SyntacticallyParsedConstructorArgumentCount: {SyntacticallyParsedConstructorArgumentCount}]");
            PropagateParsedConstructorArgument = LoggerMessage.DefineScope<string, int>("[ConstructorParameterName: {ConstructorParameterName}, ConstructorParameterIndex: {ConstructorParameterIndex}]");
            PropagateParsedConstructorArgumentOfKind = LoggerMessage.DefineScope<string>("ConstructorArgumentKind: {ConstructorArgumentKind}");

            PropagateParsedNamedArguments = LoggerMessage.DefineScope<int, int>("[SemanticallyParsedNamedArgumentCount: {SemanticallyParsedNamedArgumentCount}, SyntacticallyParsedNamedArgumentCount: {SyntacticallyParsedNamedArgumentCount}]");
            PropagateParsedNamedArgument = LoggerMessage.DefineScope<string>("[NamedParameterName: {NamedParameterName}]");
        }
    }

    private static class MessageDefinitions
    {
        public static Action<ILogger, Exception?> SemanticParserFailedToParseAttribute { get; }
        public static Action<ILogger, Exception?> SyntacticParserFailedToParseAttribute { get; }

        public static Action<ILogger, Exception?> DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments { get; }
        public static Action<ILogger, Exception?> TypeArgumentOnlyRecordedSemantically { get; }

        public static Action<ILogger, Exception?> DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments { get; }
        public static Action<ILogger, Exception?> ConstructorArgumentOnlyRecordedSemantically { get; }

        public static Action<ILogger, Exception?> DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments { get; }
        public static Action<ILogger, Exception?> NamedArgumentOnlyRecordedSemantically { get; }

        static MessageDefinitions()
        {
            SemanticParserFailedToParseAttribute = LoggerMessage.Define(LogLevel.Debug, EventIDs.SemanticParserFailedToParseAttribute, "Failed to parse an attribute, as a semantic parser failed to parse the attribute.");
            SyntacticParserFailedToParseAttribute = LoggerMessage.Define(LogLevel.Debug, EventIDs.SyntacticParserFailedToParseAttribute, "Failed to parse an attribute, as a syntactic parser failed to parse the attribute.");

            DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments = LoggerMessage.Define(LogLevel.Debug, EventIDs.DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments, "Failed to parse the type arguments of an attribute, as the number of semantically parsed arguments differed from the number of syntactically parsed arguments.");
            TypeArgumentOnlyRecordedSemantically = LoggerMessage.Define(LogLevel.Debug, EventIDs.TypeArgumentOnlyRecordedSemantically, "Failed to parse a type argument, as the argument was parsed semantically but not syntactically.");

            DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments = LoggerMessage.Define(LogLevel.Debug, EventIDs.DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments, "Failed to parse the constructor arguments of an attribute, as the number of semantically parsed arguments differed from the number of syntactically parsed arguments.");
            ConstructorArgumentOnlyRecordedSemantically = LoggerMessage.Define(LogLevel.Debug, EventIDs.ConstructorArgumentOnlyRecordedSemantically, "Failed to parse a constructor argument, as the argument was parsed semantically but not syntactically.");

            DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments = LoggerMessage.Define(LogLevel.Debug, EventIDs.DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments, "Failed to parse the named arguments of an attribute, as the number of semantically parsed arguments differed from the number of syntactically parsed arguments.");
            NamedArgumentOnlyRecordedSemantically = LoggerMessage.Define(LogLevel.Debug, EventIDs.NamedArgumentOnlyRecordedSemantically, "Failed to parse a named argument, as the argument was parsed semantically but not syntactically.");
        }
    }

    private static class EventIDs
    {
        public static EventId SemanticParserFailedToParseAttribute { get; }
        public static EventId SyntacticParserFailedToParseAttribute { get; }

        public static EventId DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments { get; }
        public static EventId TypeArgumentOnlyRecordedSemantically { get; }

        public static EventId DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments { get; }
        public static EventId ConstructorArgumentOnlyRecordedSemantically { get; }

        public static EventId DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments { get; }
        public static EventId NamedArgumentOnlyRecordedSemantically { get; }

        static EventIDs()
        {
            SequentialEventID eventIDs = new();

            SemanticParserFailedToParseAttribute = new(eventIDs.Next, nameof(SemanticParserFailedToParseAttribute));
            SyntacticParserFailedToParseAttribute = new(eventIDs.Next, nameof(SyntacticParserFailedToParseAttribute));

            DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments = new(eventIDs.Next, nameof(DifferentNumberOfSemanticallyAndSyntacticallyParsedTypeArguments));
            TypeArgumentOnlyRecordedSemantically = new(eventIDs.Next, nameof(TypeArgumentOnlyRecordedSemantically));

            DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments = new(eventIDs.Next, nameof(DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments));
            ConstructorArgumentOnlyRecordedSemantically = new(eventIDs.Next, nameof(ConstructorArgumentOnlyRecordedSemantically));

            DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments = new(eventIDs.Next, nameof(DifferentNumberOfSemanticallyAndSyntacticallyParsedNamedArguments));
            NamedArgumentOnlyRecordedSemantically = new(eventIDs.Next, nameof(NamedArgumentOnlyRecordedSemantically));
        }
    }
}
