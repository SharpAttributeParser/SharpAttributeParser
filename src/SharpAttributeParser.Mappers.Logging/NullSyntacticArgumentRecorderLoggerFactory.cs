namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.Extensions.Logging;

/// <summary>A <see cref="ISyntacticArgumentRecorderLoggerFactory"/> creating <see cref="ISyntacticArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
public sealed class NullSyntacticArgumentRecorderLoggerFactory : ISyntacticArgumentRecorderLoggerFactory
{
    /// <summary>The singleton <see cref="NullSyntacticArgumentRecorderLoggerFactory"/>.</summary>
    public static NullSyntacticArgumentRecorderLoggerFactory Singleton { get; } = new();

    private NullSyntacticArgumentRecorderLoggerFactory() { }

    ISyntacticArgumentRecorderLogger<TCategoryName> ISyntacticArgumentRecorderLoggerFactory.Create<TCategoryName>() => NullSyntacticArgumentRecorderLogger<TCategoryName>.Singleton;
    ISyntacticArgumentRecorderLogger<TCategoryName> ISyntacticArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger) => NullSyntacticArgumentRecorderLogger<TCategoryName>.Singleton;
}
