namespace SharpAttributeParser.Recording;

public interface ISyntacticArrayConstructorAttributeRecorderFactory
{
    public abstract ISyntacticAttributeRecorder<ISyntacticArrayConstructorAttributeRecord> Create();
}
