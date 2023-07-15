namespace SharpAttributeParser.Recording;

public interface ICombinedAttributeRecorderFactory
{
    public abstract IAttributeRecorder<ICombinedAttributeRecord> Create();
}
