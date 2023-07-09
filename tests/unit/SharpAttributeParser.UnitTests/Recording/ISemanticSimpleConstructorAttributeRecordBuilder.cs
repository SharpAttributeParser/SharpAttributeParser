namespace SharpAttributeParser.Recording;

internal interface ISemanticSimpleConstructorAttributeRecordBuilder : IRecordBuilder<ISemanticSimpleConstructorAttributeRecord>
{
    public abstract void WithValue(object? value);
}
