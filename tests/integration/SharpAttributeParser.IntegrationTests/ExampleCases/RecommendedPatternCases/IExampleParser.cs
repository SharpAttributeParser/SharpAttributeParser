namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases;

using Microsoft.CodeAnalysis;

public interface IExampleParser
{
    public abstract IExampleRecord? TryParse(AttributeData attributeData);
}
