namespace SharpAttributeParser.Recording;

public interface ISemanticCombinedAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ICombinedAttributeData> Create();
}
