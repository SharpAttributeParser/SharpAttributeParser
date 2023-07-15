namespace SharpAttributeParser.Recording;

public interface ISemanticGenericAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ISemanticGenericAttributeRecord> Create();
}
