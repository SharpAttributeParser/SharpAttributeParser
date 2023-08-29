namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.Extensions.Logging;

using System;

/// <inheritdoc cref="ISemanticArgumentRecorderLoggerFactory"/>
public sealed class SemanticArgumentRecorderLoggerFactory : ISemanticArgumentRecorderLoggerFactory
{
    private ILoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="SemanticArgumentRecorderLoggerFactory"/>, handling creation of <see cref="ISemanticArgumentRecorderLogger"/>.</summary>
    /// <param name="loggerFactory">Handles creation of loggers.</param>
    /// <exception cref="ArgumentNullException"/>
    public SemanticArgumentRecorderLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    ISemanticArgumentRecorderLogger<TCategoryName> ISemanticArgumentRecorderLoggerFactory.Create<TCategoryName>()
    {
        var logger = LoggerFactory.CreateLogger<TCategoryName>();

        return Create(logger);
    }

    ISemanticArgumentRecorderLogger<TCategoryName> ISemanticArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return Create(logger);
    }

    private static ISemanticArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger) => new SemanticArgumentRecorderLogger<TCategoryName>(logger);
}
