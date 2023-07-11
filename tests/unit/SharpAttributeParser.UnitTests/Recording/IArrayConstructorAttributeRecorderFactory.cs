namespace SharpAttributeParser.Recording;

public interface IArrayConstructorAttributeRecorderFactory
{
    public abstract IAttributeRecorder<IArrayConstructorAttributeRecord> Create();
}
