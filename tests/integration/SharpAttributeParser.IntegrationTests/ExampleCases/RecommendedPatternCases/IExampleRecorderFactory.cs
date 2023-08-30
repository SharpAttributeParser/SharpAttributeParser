namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases;

public interface IExampleRecorderFactory
{
    public abstract ISemanticRecorder<IExampleRecord> Create();
}
