namespace SharpAttributeParser.Recording;

public interface IQualifiedGenericAttributeRecorderFactory
{
    public abstract IAttributeRecorder<IQualifiedGenericAttributeRecord> Create();
}
