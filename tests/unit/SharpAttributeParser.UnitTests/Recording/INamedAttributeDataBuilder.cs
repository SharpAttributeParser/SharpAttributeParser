namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal interface INamedAttributeDataBuilder : IAttributeDataBuilder<INamedAttributeData>
{
    public abstract void WithSimpleValue(object? value, Location location);
    public abstract void WithArrayValue(IReadOnlyList<object?>? value, CollectionLocation location);
}
