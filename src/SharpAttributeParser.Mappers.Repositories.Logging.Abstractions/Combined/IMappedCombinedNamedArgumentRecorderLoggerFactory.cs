namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using Microsoft.Extensions.Logging;

using System;

/// <summary>Handles creation of <see cref="IMappedCombinedNamedArgumentRecorderLogger"/>.</summary>
public interface IMappedCombinedNamedArgumentRecorderLoggerFactory
{
    /// <summary>Creates a logger.</summary>
    /// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
    /// <returns>The created logger.</returns>
    public abstract IMappedCombinedNamedArgumentRecorderLogger<TCategoryName> Create<TCategoryName>();

    /// <summary>Creates a logger.</summary>
    /// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
    /// <param name="logger">The logger used to log messages.</param>
    /// <returns>The created logger.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract IMappedCombinedNamedArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger);
}
