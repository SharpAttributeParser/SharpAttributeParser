namespace SharpAttributeParser.Recording;

public interface ISemanticQualifiedGenericAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<IQualifiedGenericAttributeData> Create();
}
