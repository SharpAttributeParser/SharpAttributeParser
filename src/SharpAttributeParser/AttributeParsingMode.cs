namespace SharpAttributeParser;

/// <summary>Describes how an attribute is parsed.</summary>
public enum AttributeParsingMode
{
    /// <summary>The <see cref="AttributeParsingMode"/> is unknown.</summary>
    Unknown,
    /// <summary>The attribute is parsed semantically.</summary>
    Semantically,
    /// <summary>The attriubte is parsed syntactically.</summary>
    Syntactically
}
