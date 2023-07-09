namespace SharpAttributeParser.Recording;

public interface ISemanticArrayConstructorAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ISemanticArrayConstructorAttributeRecord> Create();
}
