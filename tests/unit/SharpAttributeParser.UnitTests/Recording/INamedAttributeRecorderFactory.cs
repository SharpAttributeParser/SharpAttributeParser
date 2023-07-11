namespace SharpAttributeParser.Recording;

public interface INamedAttributeRecorderFactory
{
    public abstract IAttributeRecorder<INamedAttributeRecord> Create();
}
