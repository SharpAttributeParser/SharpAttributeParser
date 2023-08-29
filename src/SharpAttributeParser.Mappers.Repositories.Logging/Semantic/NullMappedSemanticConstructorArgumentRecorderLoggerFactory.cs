namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using Microsoft.Extensions.Logging;

/// <summary>A <see cref="IMappedSemanticConstructorArgumentRecorderLoggerFactory"/> creating <see cref="IMappedSemanticConstructorArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
public sealed class NullMappedSemanticConstructorArgumentRecorderLoggerFactory : IMappedSemanticConstructorArgumentRecorderLoggerFactory
{
    /// <summary>The singleton <see cref="NullMappedSemanticConstructorArgumentRecorderLoggerFactory"/>.</summary>
    public static NullMappedSemanticConstructorArgumentRecorderLoggerFactory Singleton { get; } = new();

    private NullMappedSemanticConstructorArgumentRecorderLoggerFactory() { }

    IMappedSemanticConstructorArgumentRecorderLogger<TCategoryName> IMappedSemanticConstructorArgumentRecorderLoggerFactory.Create<TCategoryName>() => NullMappedSemanticConstructorArgumentRecorderLogger<TCategoryName>.Singleton;
    IMappedSemanticConstructorArgumentRecorderLogger<TCategoryName> IMappedSemanticConstructorArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger) => NullMappedSemanticConstructorArgumentRecorderLogger<TCategoryName>.Singleton;
}
