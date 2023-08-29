namespace SharpAttributeParser.Mappers.Repositories.Logging.Combined;

using SharpAttributeParser.Mappers.Repositories.Combined;

using System.Diagnostics.CodeAnalysis;

/// <summary>Handles logging for <see cref="IDetachedMappedCombinedConstructorArgumentRecorder{TRecord}"/></summary>
public interface IMappedCombinedConstructorArgumentRecorderLogger
{
    /// <summary>Logs a message describing a constructor argument not being recorded, as it did not follow some specified pattern.</summary>
    public abstract void ConstructorArgumentNotFollowingPattern();
}

/// <inheritdoc cref="IMappedCombinedConstructorArgumentRecorderLogger"/>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Follows the pattern of ILogger<CategoryName>")]
public interface IMappedCombinedConstructorArgumentRecorderLogger<out TCategoryName> : IMappedCombinedConstructorArgumentRecorderLogger { }
