namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.Extensions.Logging;

using System;

/// <inheritdoc cref="ICombinedArgumentRecorderLoggerFactory"/>
public sealed class CombinedArgumentRecorderLoggerFactory : ICombinedArgumentRecorderLoggerFactory
{
    private ILoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="CombinedArgumentRecorderLoggerFactory"/>, handling creation of <see cref="ICombinedArgumentRecorderLogger"/>.</summary>
    /// <param name="loggerFactory">Handles creation of loggers.</param>
    /// <exception cref="ArgumentNullException"/>
    public CombinedArgumentRecorderLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    ICombinedArgumentRecorderLogger<TCategoryName> ICombinedArgumentRecorderLoggerFactory.Create<TCategoryName>()
    {
        var logger = LoggerFactory.CreateLogger<TCategoryName>();

        return Create(logger);
    }

    ICombinedArgumentRecorderLogger<TCategoryName> ICombinedArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return Create(logger);
    }

    private static ICombinedArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger) => new CombinedArgumentRecorderLogger<TCategoryName>(logger);
}
