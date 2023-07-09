namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

public interface ISemanticArrayConstructorAttributeRecord
{
    public abstract IReadOnlyList<object?>? Value { get; }
    public abstract bool ValueRecorded { get; }
}
