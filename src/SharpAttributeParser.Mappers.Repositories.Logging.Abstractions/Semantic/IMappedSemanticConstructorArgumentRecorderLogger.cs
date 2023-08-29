namespace SharpAttributeParser.Mappers.Repositories.Logging.Semantic;

using SharpAttributeParser.Mappers.Repositories.Semantic;

using System.Diagnostics.CodeAnalysis;

/// <summary>Handles logging for <see cref="IDetachedMappedSemanticConstructorArgumentRecorder{TRecord}"/></summary>
public interface IMappedSemanticConstructorArgumentRecorderLogger
{
    /// <summary>Logs a message describing a constructor argument not being recorded, as it did not follow some specified pattern.</summary>
    public abstract void ConstructorArgumentNotFollowingPattern();
}

/// <inheritdoc cref="IMappedSemanticConstructorArgumentRecorderLogger"/>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Follows the pattern of ILogger<CategoryName>")]
public interface IMappedSemanticConstructorArgumentRecorderLogger<out TCategoryName> : IMappedSemanticConstructorArgumentRecorderLogger { }
