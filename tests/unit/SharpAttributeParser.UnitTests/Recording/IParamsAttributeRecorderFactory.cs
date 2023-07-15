namespace SharpAttributeParser.Recording;

public interface IParamsAttributeRecorderFactory
{
    public abstract IAttributeRecorder<IParamsAttributeRecord> Create();
}
