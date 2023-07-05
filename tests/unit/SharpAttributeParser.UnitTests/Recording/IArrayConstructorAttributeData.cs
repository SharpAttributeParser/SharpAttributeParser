namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

public interface IArrayConstructorAttributeData
{
    public abstract IReadOnlyList<object?>? Value { get; }
    public abstract bool ValueRecorded { get; }
    public abstract CollectionLocation ValueLocation { get; }
}
