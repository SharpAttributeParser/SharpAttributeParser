namespace SharpAttributeParser.Mappers.Logging;

using Microsoft.Extensions.Logging;

using System;

/// <summary>Handles construction of <see cref="ISemanticArgumentRecorderLogger"/>.</summary>
public interface ISemanticArgumentRecorderLoggerFactory
{
    /// <summary>Creates a logger.</summary>
    /// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
    /// <returns>The created logger.</returns>
    public abstract ISemanticArgumentRecorderLogger<TCategoryName> Create<TCategoryName>();

    /// <summary>Creates a logger.</summary>
    /// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
    /// <param name="logger">The logger used to log messages.</param>
    /// <returns>The created logger.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticArgumentRecorderLogger<TCategoryName> Create<TCategoryName>(ILogger<TCategoryName> logger);
}
