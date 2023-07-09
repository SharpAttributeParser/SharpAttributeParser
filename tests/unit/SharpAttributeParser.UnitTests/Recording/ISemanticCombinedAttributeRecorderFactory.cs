namespace SharpAttributeParser.Recording;

public interface ISemanticCombinedAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ISemanticCombinedAttributeRecord> Create();
}
