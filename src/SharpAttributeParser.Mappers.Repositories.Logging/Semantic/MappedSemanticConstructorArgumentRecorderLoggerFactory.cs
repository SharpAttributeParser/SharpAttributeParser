namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using Microsoft.Extensions.Logging;

using System;

/// <inheritdoc cref="IMappedSemanticConstructorArgumentRecorderLoggerFactory"/>
public sealed class MappedSemanticConstructorArgumentRecorderLoggerFactory : IMappedSemanticConstructorArgumentRecorderLoggerFactory
{
    private ILoggerFactory LoggerFactory { get; }

    /// <summary>Instantiates a <see cref="MappedSemanticConstructorArgumentRecorderLoggerFactory"/>, handling creation of <see cref="IMappedSemanticConstructorArgumentRecorderLogger"/>.</summary>
    /// <param name="loggerFactory">Handles creation of loggers.</param>
    public MappedSemanticConstructorArgumentRecorderLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    IMappedSemanticConstructorArgumentRecorderLogger<TCategoryName> IMappedSemanticConstructorArgumentRecorderLoggerFactory.Create<TCategoryName>()
    {
        var logger = LoggerFactory.CreateLogger<TCategoryName>();

        return Create(logger);
    }

    IMappedSemanticConstructorArgumentRecorderLogger<TCategoryName> IMappedSemanticConstructorArgumentRecorderLoggerFactory.Create<TCategoryName>(ILogger<TCategoryName> logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return Create(logger);
    }

    private static IMappedSemanticConstructorArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger) => new MappedSemanticConstructorArgumentRecorderLogger<TCategoryName>(logger);
}
