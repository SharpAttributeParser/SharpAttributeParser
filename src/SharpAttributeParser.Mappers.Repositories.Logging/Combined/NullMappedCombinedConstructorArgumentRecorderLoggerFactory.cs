namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using Microsoft.Extensions.Logging;

/// <summary>A <see cref="IMappedCombinedConstructorArgumentRecorderLoggerFactory"/> creating <see cref="IMappedCombinedConstructorArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
public sealed class NullMappedCombinedConstructorArgumentRecorderLoggerFactory : IMappedCombinedConstructorArgumentRecorderLoggerFactory
{
    /// <summary>The singleton <see cref="NullMappedCombinedConstructorArgumentRecorderLoggerFactory"/>.</summary>
    public static NullMappedCombinedConstructorArgumentRecorderLoggerFactory Singleton { get; } = new();

    private NullMappedCombinedConstructorArgumentRecorderLoggerFactory() { }

    IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> IMappedCombinedConstructorArgumentRecorderLoggerFactory.Create<TCategoryName>() => NullMappedCombinedConstructorArgumentRecorderLogger<TCategoryName>.Singleton;
    IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> IMappedCombinedConstructorArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger) => NullMappedCombinedConstructorArgumentRecorderLogger<TCategoryName>.Singleton;
}
