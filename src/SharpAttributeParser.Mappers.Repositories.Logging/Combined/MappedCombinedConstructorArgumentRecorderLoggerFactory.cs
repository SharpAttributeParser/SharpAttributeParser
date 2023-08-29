namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using Microsoft.Extensions.Logging;

using System;

/// <inheritdoc cref="IMappedCombinedConstructorArgumentRecorderLoggerFactory"/>
public sealed class MappedCombinedConstructorArgumentRecorderLoggerFactory : IMappedCombinedConstructorArgumentRecorderLoggerFactory
{
    private ILoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="MappedCombinedConstructorArgumentRecorderLoggerFactory"/>, handling creation of <see cref="IMappedCombinedConstructorArgumentRecorderLogger"/>.</summary>
    /// <param name="loggerFactory">Handles creation of loggers.</param>
    /// <exception cref="ArgumentNullException"/>
    public MappedCombinedConstructorArgumentRecorderLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> IMappedCombinedConstructorArgumentRecorderLoggerFactory.Create<TCategoryName>()
    {
        var logger = LoggerFactory.CreateLogger<TCategoryName>();

        return Create(logger);
    }

    IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> IMappedCombinedConstructorArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return Create(logger);
    }

    private static IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger) => new MappedCombinedConstructorArgumentRecorderLogger<TCategoryName>(logger);
}
