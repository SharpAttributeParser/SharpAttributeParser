namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

internal interface IArrayConstructorAttributeDataBuilder : IAttributeDataBuilder<IArrayConstructorAttributeData>
{
    public abstract void WithValue(IReadOnlyList<object?>? value, CollectionLocation location);
}
