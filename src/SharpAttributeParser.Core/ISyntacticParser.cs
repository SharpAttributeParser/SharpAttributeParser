namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Extracts syntactic information about the arguments of attributes.</summary>
public interface ISyntacticParser
{
    /// <summary>Attempts to extract syntactic information about the arguments of an attribute.</summary>
    /// <param name="recorder">The recorder responsible for recording the syntactic information.</param>
    /// <param name="attributeData">The semantic description of the attribute.</param>
    /// <param name="attributeSyntax">The syntactic description of the attribute.</param>
    /// <returns>A <see cref="bool"/> indicating whether the attempt was successful.</returns>
    public abstract bool TryParse(ISyntacticRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax);
}
