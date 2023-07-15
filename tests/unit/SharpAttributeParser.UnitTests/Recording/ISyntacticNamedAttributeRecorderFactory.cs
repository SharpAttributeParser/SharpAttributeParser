namespace SharpAttributeParser.Recording;

public interface ISyntacticNamedAttributeRecorderFactory
{
    public abstract ISyntacticAttributeRecorder<ISyntacticNamedAttributeRecord> Create();
}
