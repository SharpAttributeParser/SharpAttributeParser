namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using Microsoft.Extensions.Logging;

using System;

/// <inheritdoc cref="IMappedCombinedNamedArgumentRecorderLoggerFactory"/>
public sealed class MappedCombinedNamedArgumentRecorderLoggerFactory : IMappedCombinedNamedArgumentRecorderLoggerFactory
{
    private ILoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="MappedCombinedNamedArgumentRecorderLoggerFactory"/>, handling creation of <see cref="IMappedCombinedNamedArgumentRecorderLogger"/>.</summary>
    /// <param name="loggerFactory">Handles creation of loggers.</param>
    /// <exception cref="ArgumentNullException"/>
    public MappedCombinedNamedArgumentRecorderLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    IMappedCombinedNamedArgumentRecorderLogger<TCategoryName> IMappedCombinedNamedArgumentRecorderLoggerFactory.Create<TCategoryName>()
    {
        var logger = LoggerFactory.CreateLogger<TCategoryName>();

        return Create(logger);
    }

    IMappedCombinedNamedArgumentRecorderLogger<TCategoryName> IMappedCombinedNamedArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return Create(logger);
    }

    private static IMappedCombinedNamedArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger) => new MappedCombinedNamedArgumentRecorderLogger<TCategoryName>(logger);
}
