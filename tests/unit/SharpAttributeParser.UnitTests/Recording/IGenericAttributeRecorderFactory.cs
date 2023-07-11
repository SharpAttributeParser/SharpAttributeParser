namespace SharpAttributeParser.Recording;

public interface IGenericAttributeRecorderFactory
{
    public abstract IAttributeRecorder<IGenericAttributeRecord> Create();
}
