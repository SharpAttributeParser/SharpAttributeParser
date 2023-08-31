namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Parses the arguments of attributes, and extracts syntactic information about the arguments.</summary>
public interface ICombinedParser
{
    /// <summary>Attempts to parse the arguments of an attribute, and extract syntactic information about the arguments.</summary>
    /// <param name="recorder">The recorder responsible for recording the parsed arguments and syntactic information.</param>
    /// <param name="attributeData">The semantic description of the attribute.</param>
    /// <param name="attributeSyntax">The syntactic description of the attribute.</param>
    /// <returns>A <see cref="bool"/> indicating whether the attempt was successful.</returns>
    public abstract bool TryParse(ICombinedRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax);
}
