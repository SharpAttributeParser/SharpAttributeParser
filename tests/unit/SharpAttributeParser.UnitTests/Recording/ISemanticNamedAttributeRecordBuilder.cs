namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

internal interface ISemanticNamedAttributeRecordBuilder : IRecordBuilder<ISemanticNamedAttributeRecord>
{
    public abstract void WithSimpleValue(object? value);
    public abstract void WithArrayValue(IReadOnlyList<object?>? value);
}
