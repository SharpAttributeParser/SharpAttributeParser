namespace SharpAttributeParser.Recording;

public interface ISyntacticQualifiedGenericAttributeRecorderFactory
{
    public abstract ISyntacticAttributeRecorder<ISyntacticQualifiedGenericAttributeRecord> Create();
}
