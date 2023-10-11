namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.Extensions.Logging;

using System;

/// <inheritdoc cref="ISyntacticArgumentRecorderLoggerFactory"/>
public sealed class SyntacticArgumentRecorderLoggerFactory : ISyntacticArgumentRecorderLoggerFactory
{
    private readonly ILoggerFactory LoggerFactory;

    /// <summary>Instantiates a <see cref="SyntacticArgumentRecorderLoggerFactory"/>, handling creation of <see cref="ISyntacticArgumentRecorderLogger"/>.</summary>
    /// <param name="loggerFactory">Handles creation of loggers.</param>
    public SyntacticArgumentRecorderLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    ISyntacticArgumentRecorderLogger<TCategoryName> ISyntacticArgumentRecorderLoggerFactory.Create<TCategoryName>()
    {
        var logger = LoggerFactory.CreateLogger<TCategoryName>();

        return Create(logger);
    }

    ISyntacticArgumentRecorderLogger<TCategoryName> ISyntacticArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return Create(logger);
    }

    private static ISyntacticArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger) => new SyntacticArgumentRecorderLogger<TCategoryName>(logger);
}
