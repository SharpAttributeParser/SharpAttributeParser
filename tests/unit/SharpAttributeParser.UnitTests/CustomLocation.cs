namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using Moq;

internal static class CustomLocation
{
    public static Location Create() => Mock.Of<Location>();
    public static CollectionLocation CreateCollection() => CollectionLocation.Empty(Create());
}
