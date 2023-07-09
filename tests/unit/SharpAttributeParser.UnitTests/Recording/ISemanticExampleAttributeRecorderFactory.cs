namespace SharpAttributeParser.Recording;

public interface ISemanticExampleAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ISemanticExampleAttributeRecord> Create();
}
