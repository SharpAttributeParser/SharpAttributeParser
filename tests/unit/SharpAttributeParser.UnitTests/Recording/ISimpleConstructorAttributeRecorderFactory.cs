namespace SharpAttributeParser.Recording;

public interface ISimpleConstructorAttributeRecorderFactory
{
    public abstract IAttributeRecorder<ISimpleConstructorAttributeRecord> Create();
}
