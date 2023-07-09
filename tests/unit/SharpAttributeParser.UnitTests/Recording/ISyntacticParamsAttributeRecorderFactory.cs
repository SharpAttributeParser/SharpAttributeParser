namespace SharpAttributeParser.Recording;

public interface ISyntacticParamsAttributeRecorderFactory
{
    public abstract ISyntacticAttributeRecorder<ISyntacticParamsAttributeRecord> Create();
}
