namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

internal interface ISimpleConstructorAttributeDataBuilder : IAttributeDataBuilder<ISimpleConstructorAttributeData>
{
    public abstract void WithValue(object? value, Location location);
}
