namespace SharpAttributeParser.Recording;

public interface ISemanticSimpleConstructorAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ISemanticSimpleConstructorAttributeRecord> Create();
}
