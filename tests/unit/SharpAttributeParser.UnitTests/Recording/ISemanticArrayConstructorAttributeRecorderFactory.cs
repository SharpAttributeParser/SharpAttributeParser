namespace SharpAttributeParser.Recording;

public interface ISemanticArrayConstructorAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<IArrayConstructorAttributeData> Create();
}
