namespace SharpAttributeParser.Recording;

public interface ISyntacticSimpleConstructorAttributeRecorderFactory
{
    public abstract ISyntacticAttributeRecorder<ISyntacticSimpleConstructorAttributeRecord> Create();
}
