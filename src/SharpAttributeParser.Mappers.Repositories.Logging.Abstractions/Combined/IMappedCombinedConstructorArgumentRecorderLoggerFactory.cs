namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using Microsoft.Extensions.Logging;

/// <summary>Handles creation of <see cref="IMappedCombinedConstructorArgumentRecorderLogger"/>.</summary>
public interface IMappedCombinedConstructorArgumentRecorderLoggerFactory
{
    /// <summary>Creates a logger.</summary>
    /// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
    /// <returns>The created logger.</returns>
    public abstract IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> Create<TCategoryName>();

    /// <summary>Creates a logger.</summary>
    /// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
    /// <param name="logger">The logger used to log messages.</param>
    /// <returns>The created logger.</returns>
    public abstract IMappedCombinedConstructorArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger);
}
