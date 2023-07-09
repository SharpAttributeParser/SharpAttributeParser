namespace SharpAttributeParser.Recording;

public interface ISemanticQualifiedGenericAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ISemanticQualifiedGenericAttributeRecord> Create();
}
