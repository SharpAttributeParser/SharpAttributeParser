namespace SharpAttributeParser.Recording;

public interface ISyntacticGenericAttributeRecorderFactory
{
    public abstract ISyntacticAttributeRecorder<ISyntacticGenericAttributeRecord> Create();
}
