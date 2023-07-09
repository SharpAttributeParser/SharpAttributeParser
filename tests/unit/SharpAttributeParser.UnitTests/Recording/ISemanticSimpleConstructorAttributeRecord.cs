namespace SharpAttributeParser.Recording;

public interface ISemanticSimpleConstructorAttributeRecord
{
    public abstract object? Value { get; }
    public abstract bool ValueRecorded { get; }
}
