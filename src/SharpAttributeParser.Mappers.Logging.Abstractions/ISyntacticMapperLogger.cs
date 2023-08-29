namespace SharpAttributeParser.Mappers.Logging;

using SharpAttributeParser.Mappers.Logging.SyntacticMapperComponents;

using System.Diagnostics.CodeAnalysis;

/// <summary>Handles logging for <see cref="ISyntacticMapper{TRecord}"/>.</summary>
public interface ISyntacticMapperLogger
{
    /// <summary>Handles logging related to type parameters.</summary>
    public abstract ITypeParameterLogger TypeParameter { get; }

    /// <summary>Handles logging related to constructor parameters.</summary>
    public abstract IConstructorParameterLogger ConstructorParameter { get; }

    /// <summary>Handles logging related to named parameters.</summary>
    public abstract INamedParameterLogger NamedParameter { get; }
}

/// <inheritdoc cref="ISyntacticMapperLogger"/>
/// <typeparam name="TCategoryName">The name of the logging category.</typeparam>
[SuppressMessage("Major Code Smell", "S2326: Unused type parameters should be removed", Justification = "Follows the pattern of ILogger<CategoryName>")]
public interface ISyntacticMapperLogger<out TCategoryName> : ISyntacticMapperLogger { }
