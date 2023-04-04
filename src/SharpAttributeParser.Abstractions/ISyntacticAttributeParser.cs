namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Parses the arguments of an attribute syntactically.</summary>
public interface ISyntacticAttributeParser
{
    /// <summary>Attempts to parse the arguments of an attribute, described by the provided <see cref="AttributeData"/>.</summary>
    /// <param name="recorder">Responsible for recording the parsed arguments.</param>
    /// <param name="attributeData">The attribute being parsed.</param>
    /// <param name="attributeSyntax">Syntactical information about the attribute being parsed.</param>
    /// <returns>A <see cref="bool"/> indicating whether the attempt was deemed successful.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryParse(ISyntacticArgumentRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax);
}
