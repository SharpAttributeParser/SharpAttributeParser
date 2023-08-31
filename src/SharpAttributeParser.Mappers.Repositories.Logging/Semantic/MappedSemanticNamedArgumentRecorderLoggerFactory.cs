namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using Microsoft.Extensions.Logging;

using System;

/// <inheritdoc cref="IMappedSemanticNamedArgumentRecorderLoggerFactory"/>
public sealed class MappedSemanticNamedArgumentRecorderLoggerFactory : IMappedSemanticNamedArgumentRecorderLoggerFactory
{
    private ILoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="MappedSemanticNamedArgumentRecorderLoggerFactory"/>, handling creation of <see cref="IMappedSemanticNamedArgumentRecorderLogger"/>.</summary>
    /// <param name="loggerFactory">Handles creation of loggers.</param>
    public MappedSemanticNamedArgumentRecorderLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    IMappedSemanticNamedArgumentRecorderLogger<TCategoryName> IMappedSemanticNamedArgumentRecorderLoggerFactory.Create<TCategoryName>()
    {
        var logger = LoggerFactory.CreateLogger<TCategoryName>();

        return Create(logger);
    }

    IMappedSemanticNamedArgumentRecorderLogger<TCategoryName> IMappedSemanticNamedArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return Create(logger);
    }

    private static IMappedSemanticNamedArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger) => new MappedSemanticNamedArgumentRecorderLogger<TCategoryName>(logger);
}
