namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.Extensions.Logging;

/// <summary>A <see cref="ISemanticArgumentRecorderLoggerFactory"/> creating <see cref="ISemanticArgumentRecorderLogger{TCategoryName}"/> with no behaviour.</summary>
public sealed class NullSemanticArgumentRecorderLoggerFactory : ISemanticArgumentRecorderLoggerFactory
{
    /// <summary>The singleton <see cref="NullSemanticArgumentRecorderLoggerFactory"/>.</summary>
    public static NullSemanticArgumentRecorderLoggerFactory Singleton { get; } = new();

    private NullSemanticArgumentRecorderLoggerFactory() { }

    ISemanticArgumentRecorderLogger<TCategoryName> ISemanticArgumentRecorderLoggerFactory.Create<TCategoryName>() => NullSemanticArgumentRecorderLogger<TCategoryName>.Singleton;
    ISemanticArgumentRecorderLogger<TCategoryName> ISemanticArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger) => NullSemanticArgumentRecorderLogger<TCategoryName>.Singleton;
}
