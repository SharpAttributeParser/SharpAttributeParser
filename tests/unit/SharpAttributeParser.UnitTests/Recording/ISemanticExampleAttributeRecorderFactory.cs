namespace SharpAttributeParser.Recording;

public interface ISemanticExampleAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<IExampleAttributeData> Create();
}
