namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.Extensions.Logging;

/// <summary>A <see cref="ICombinedArgumentRecorderLoggerFactory"/> creating <see cref="ICombinedArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
public sealed class NullCombinedArgumentRecorderLoggerFactory : ICombinedArgumentRecorderLoggerFactory
{
    /// <summary>The singleton <see cref="NullCombinedArgumentRecorderLoggerFactory"/>.</summary>
    public static NullCombinedArgumentRecorderLoggerFactory Singleton { get; } = new();

    private NullCombinedArgumentRecorderLoggerFactory() { }

    ICombinedArgumentRecorderLogger<TCategoryName> ICombinedArgumentRecorderLoggerFactory.Create<TCategoryName>() => NullCombinedArgumentRecorderLogger<TCategoryName>.Singleton;
    ICombinedArgumentRecorderLogger<TCategoryName> ICombinedArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger) => NullCombinedArgumentRecorderLogger<TCategoryName>.Singleton;
}
