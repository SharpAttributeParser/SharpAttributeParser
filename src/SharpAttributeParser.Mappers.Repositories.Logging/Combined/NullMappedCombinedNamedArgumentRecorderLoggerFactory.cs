namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using Microsoft.Extensions.Logging;

/// <summary>A <see cref="IMappedCombinedNamedArgumentRecorderLoggerFactory"/> creating <see cref="IMappedCombinedNamedArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
public sealed class NullMappedCombinedNamedArgumentRecorderLoggerFactory : IMappedCombinedNamedArgumentRecorderLoggerFactory
{
    /// <summary>The singleton <see cref="NullMappedCombinedNamedArgumentRecorderLoggerFactory"/>.</summary>
    public static NullMappedCombinedNamedArgumentRecorderLoggerFactory Singleton { get; } = new();

    private NullMappedCombinedNamedArgumentRecorderLoggerFactory() { }

    IMappedCombinedNamedArgumentRecorderLogger<TCategoryName> IMappedCombinedNamedArgumentRecorderLoggerFactory.Create<TCategoryName>() => NullMappedCombinedNamedArgumentRecorderLogger<TCategoryName>.Singleton;
    IMappedCombinedNamedArgumentRecorderLogger<TCategoryName> IMappedCombinedNamedArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger) => NullMappedCombinedNamedArgumentRecorderLogger<TCategoryName>.Singleton;
}
