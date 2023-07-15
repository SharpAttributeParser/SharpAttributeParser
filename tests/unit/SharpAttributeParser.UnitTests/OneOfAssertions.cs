namespace SharpAttributeParser;

using OneOf;

using Xunit;

internal static class OneOfAssertions
{
    public static void Equal<T1, T2>(OneOf<T1, T2> expected, OneOf<T1, T2> actual) => expected.Switch
    (
        (t1) => Assert.Equal(t1, actual.AsT0),
        (t2) => Assert.Equal(t2, actual.AsT1)
    );
}
