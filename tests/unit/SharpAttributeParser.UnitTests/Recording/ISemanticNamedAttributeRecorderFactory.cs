namespace SharpAttributeParser.Recording;

public interface ISemanticNamedAttributeRecorderFactory
{
    public abstract ISemanticAttributeRecorder<ISemanticNamedAttributeRecord> Create();
}
