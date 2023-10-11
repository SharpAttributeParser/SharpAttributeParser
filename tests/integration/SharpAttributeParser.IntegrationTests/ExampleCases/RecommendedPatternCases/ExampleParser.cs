namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases;

using Microsoft.CodeAnalysis;

public sealed class ExampleParser : IExampleParser
{
    private readonly ISemanticParser Parser;
    private readonly IExampleRecorderFactory RecorderFactory;

    public ExampleParser(ISemanticParser parser, IExampleRecorderFactory recorderFactory)
    {
        Parser = parser;
        RecorderFactory = recorderFactory;
    }

    IExampleRecord? IExampleParser.TryParse(AttributeData attributeData)
    {
        var recorder = RecorderFactory.Create();

        if (Parser.TryParse(recorder, attributeData) is false)
        {
            return null;
        }

        return recorder.GetRecord();
    }
}
