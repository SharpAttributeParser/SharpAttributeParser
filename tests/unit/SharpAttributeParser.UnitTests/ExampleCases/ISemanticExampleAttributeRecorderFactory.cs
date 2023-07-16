namespace SharpAttributeParser.ExampleCases;
public interface ISemanticExampleAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ISemanticExampleAttributeRecord> Create();
}
