namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using Microsoft.Extensions.Logging;

/// <summary>A <see cref="IMappedSemanticNamedArgumentRecorderLoggerFactory"/> creating <see cref="IMappedSemanticNamedArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
public sealed class NullMappedSemanticNamedArgumentRecorderLoggerFactory : IMappedSemanticNamedArgumentRecorderLoggerFactory
{
    /// <summary>The singleton <see cref="NullMappedSemanticNamedArgumentRecorderLoggerFactory"/>.</summary>
    public static NullMappedSemanticNamedArgumentRecorderLoggerFactory Singleton { get; } = new();

    private NullMappedSemanticNamedArgumentRecorderLoggerFactory() { }

    IMappedSemanticNamedArgumentRecorderLogger<TCategoryName> IMappedSemanticNamedArgumentRecorderLoggerFactory.Create<TCategoryName>() => NullMappedSemanticNamedArgumentRecorderLogger<TCategoryName>.Singleton;
    IMappedSemanticNamedArgumentRecorderLogger<TCategoryName> IMappedSemanticNamedArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger) => NullMappedSemanticNamedArgumentRecorderLogger<TCategoryName>.Singleton;
}
