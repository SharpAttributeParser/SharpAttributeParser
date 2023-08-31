namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using Microsoft.Extensions.Logging;

/// <summary>Handles creation of <see cref="IMappedSemanticNamedArgumentRecorderLogger"/>.</summary>
public interface IMappedSemanticNamedArgumentRecorderLoggerFactory
{
    /// <summary>Creates a logger.</summary>
    /// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
    /// <returns>The created logger.</returns>
    public abstract IMappedSemanticNamedArgumentRecorderLogger<TCategoryName> Create<TCategoryName>();

    /// <summary>Creates a logger.</summary>
    /// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
    /// <param name="logger">The logger used to log messages.</param>
    /// <returns>The created logger.</returns>
    public abstract IMappedSemanticNamedArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger);
}
