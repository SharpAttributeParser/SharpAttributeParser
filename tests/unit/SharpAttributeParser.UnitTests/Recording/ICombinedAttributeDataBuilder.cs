namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal interface ICombinedAttributeDataBuilder : IAttributeDataBuilder<ICombinedAttributeData>
{
    public abstract void WithT1(ITypeSymbol t1, Location location);
    public abstract void WithT2(ITypeSymbol t2, Location location);

    public abstract void WithSimpleValue(object? value, Location location);
    public abstract void WithArrayValue(IReadOnlyList<object?>? value, CollectionLocation location);
    public abstract void WithParamsValue(IReadOnlyList<object?>? value, CollectionLocation location);

    public abstract void WithSimpleNamedValue(object? value, Location location);
    public abstract void WithArrayNamedValue(IReadOnlyList<object?>? value, CollectionLocation location);
}
