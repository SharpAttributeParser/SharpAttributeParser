namespace SharpAttributeParser.Recording;

public interface ISemanticGenericAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<IGenericAttributeData> Create();
}
