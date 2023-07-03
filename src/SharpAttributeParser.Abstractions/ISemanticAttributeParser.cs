namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Parses the arguments of an attribute.</summary>
public interface ISemanticAttributeParser
{
    /// <summary>Attempts to parse the arguments of an attribute, described by the provided <see cref="AttributeData"/>.</summary>
    /// <param name="recorder">The <see cref="ISemanticAttributeRecorder"/>, responsible for recording the parsed arguments.</param>
    /// <param name="attributeData">The attribute being parsed.</param>
    /// <returns>A <see cref="bool"/> indicating whether the attempt was successful.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryParse(ISemanticAttributeRecorder recorder, AttributeData attributeData);
}
