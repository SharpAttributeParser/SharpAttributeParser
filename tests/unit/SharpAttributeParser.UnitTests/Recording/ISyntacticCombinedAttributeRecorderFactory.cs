namespace SharpAttributeParser.Recording;

public interface ISyntacticCombinedAttributeRecorderFactory
{
    public abstract ISyntacticAttributeRecorder<ISyntacticCombinedAttributeRecord> Create();
}
