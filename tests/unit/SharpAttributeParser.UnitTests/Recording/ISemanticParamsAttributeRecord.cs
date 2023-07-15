namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

public interface ISemanticParamsAttributeRecord
{
    public abstract IReadOnlyList<object?>? Value { get; }
    public abstract bool ValueRecorded { get; }
}
