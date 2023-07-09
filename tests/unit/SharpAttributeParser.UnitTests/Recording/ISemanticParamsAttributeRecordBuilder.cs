namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

internal interface ISemanticParamsAttributeRecordBuilder : IRecordBuilder<ISemanticParamsAttributeRecord>
{
    public abstract void WithValue(IReadOnlyList<object?>? value);
}
