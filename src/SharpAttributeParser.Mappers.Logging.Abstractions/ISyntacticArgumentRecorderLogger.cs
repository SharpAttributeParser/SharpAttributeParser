namespace SharpAttributeParser.Mappers.Logging;

using SharpAttributeParser.Mappers.Logging.SyntacticArgumentRecorderComponents;

using System.Diagnostics.CodeAnalysis;

/// <summary>Handles logging for <see cref="ISyntacticRecorder"/>.</summary>
public interface ISyntacticArgumentRecorderLogger
{
    /// <summary>Handles logging related to type arguments.</summary>
    public abstract ITypeArgumentsLogger TypeArgument { get; }

    /// <summary>Handles logging related to constructor arguments.</summary>
    public abstract IConstructorArgumentsLogger ConstructorArgument { get; }

    /// <summary>Handles logging related to named arguments.</summary>
    public abstract INamedArgumentsLogger NamedArgument { get; }
}

/// <inheritdoc cref="ISyntacticArgumentRecorderLogger"/>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Follows the pattern of ILogger<CategoryName>")]
public interface ISyntacticArgumentRecorderLogger<out TCategoryName> : ISyntacticArgumentRecorderLogger { }
