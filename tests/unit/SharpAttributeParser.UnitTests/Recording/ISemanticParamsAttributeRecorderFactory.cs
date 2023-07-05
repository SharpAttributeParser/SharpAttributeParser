namespace SharpAttributeParser.Recording;

public interface ISemanticParamsAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<IParamsAttributeData> Create();
}
