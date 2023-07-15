namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Extracts syntactical information about attribute arguments.</summary>
public interface ISyntacticAttributeParser
{
    /// <summary>Attempts to extract syntactical information about the arguments of an attribute, described by the provided <see cref="AttributeData"/> and <see cref="AttributeSyntax"/>.</summary>
    /// <param name="recorder">The <see cref="ISyntacticAttributeRecorder"/>, responsible for recording syntactical information.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> describing the attribute being parsed.</param>
    /// <param name="attributeSyntax">The <see cref="AttributeSyntax"/>, syntactically describing the attribute.</param>
    /// <returns>A <see cref="bool"/> indicating whether the attempt was successful.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryParse(ISyntacticAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax);
}
