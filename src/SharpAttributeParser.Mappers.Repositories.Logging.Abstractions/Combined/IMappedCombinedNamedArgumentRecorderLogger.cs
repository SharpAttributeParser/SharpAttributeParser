namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System.Diagnostics.CodeAnalysis;

/// <summary>Handles logging for <see cref="IDetachedMappedCombinedNamedArgumentRecorder{TRecord}"/></summary>
public interface IMappedCombinedNamedArgumentRecorderLogger
{
    /// <summary>Logs a message describing a named argument not being recorded, as it did not follow some specified pattern.</summary>
    public abstract void NamedArgumentNotFollowingPattern();
}

/// <inheritdoc cref="IMappedCombinedNamedArgumentRecorderLogger"/>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Follows the pattern of ILogger<CategoryName>")]
public interface IMappedCombinedNamedArgumentRecorderLogger<out TCategoryName> : IMappedCombinedNamedArgumentRecorderLogger { }
