namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using SharpAttributeParser.Mappers.Repositories.Semantic;

using System.Diagnostics.CodeAnalysis;

/// <summary>Handles logging for <see cref="IDetachedMappedSemanticNamedArgumentRecorder{TRecord}"/></summary>
public interface IMappedSemanticNamedArgumentRecorderLogger
{
    /// <summary>Logs a message describing a named argument not being recorded, as it did not follow some specified pattern.</summary>
    public abstract void NamedArgumentNotFollowingPattern();
}

/// <inheritdoc cref="IMappedSemanticNamedArgumentRecorderLogger"/>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Follows the pattern of ILogger<CategoryName>")]
public interface IMappedSemanticNamedArgumentRecorderLogger<out TCategoryName> : IMappedSemanticNamedArgumentRecorderLogger { }
