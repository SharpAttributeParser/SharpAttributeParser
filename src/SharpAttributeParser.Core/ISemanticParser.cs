﻿namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

/// <summary>Parses the arguments of attributes without syntactic context.</summary>
public interface ISemanticParser
{
    /// <summary>Attempts to parse the arguments of an attribute without syntactic context.</summary>
    /// <param name="recorder">The recorder responsible for recording the parsed arguments.</param>
    /// <param name="attributeData">The semantic description of the attribute.</param>
    /// <returns>A <see cref="bool"/> indicating whether the attempt was successful.</returns>
    public abstract bool TryParse(ISemanticRecorder recorder, AttributeData attributeData);
}
