namespace SharpAttributeParser.Recording;

using System.Collections.Generic;

internal interface IParamsAttributeDataBuilder : IAttributeDataBuilder<IParamsAttributeData>
{
    public abstract void WithValue(IReadOnlyList<object?>? value, CollectionLocation location);
}
