namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Parses attribute arguments, and extracts syntactical information about the arguments.</summary>
public interface IAttributeParser
{
    /// <summary>Attempts to parse the arguments of an attribute, described by the provided <see cref="AttributeData"/>.</summary>
    /// <param name="recorder">The <see cref="IAttributeRecorder"/> responsible for recording the parsed arguments and syntactical information.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> describing the attribute being parsed.</param>
    /// <param name="attributeSyntax">The <see cref="AttributeSyntax"/> syntactically describing the attribute.</param>
    /// <returns>A <see cref="bool"/> indicating whether the attempt was successful.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryParse(IAttributeRecorder recorder, AttributeData attributeData, AttributeSyntax attributeSyntax);

    /// <summary>Attempts to parse the arguments of an attribute, described by the provided <see cref="AttributeData"/>.</summary>
    /// <param name="semanticRecorder">The <see cref="ISemanticAttributeRecorder"/> responsible for recording the parsed arguments.</param>
    /// <param name="syntacticRecorder">The <see cref="ISyntacticAttributeRecorder"/> responsible for recording the extracted syntactical information.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> describing the attribute being parsed.</param>
    /// <param name="attributeSyntax">The <see cref="AttributeSyntax"/> syntactically describing the attribute.</param>
    /// <returns>A <see cref="bool"/> indicating whether the attempt was successful.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryParse(ISemanticAttributeRecorder semanticRecorder, ISyntacticAttributeRecorder syntacticRecorder, AttributeData attributeData, AttributeSyntax attributeSyntax);
}
