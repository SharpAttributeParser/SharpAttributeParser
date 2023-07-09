namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

internal interface ISemanticArrayConstructorAttributeRecordBuilder : IRecordBuilder<ISemanticArrayConstructorAttributeRecord>
{
    public abstract void WithValue(IReadOnlyList<object?>? value);
}
