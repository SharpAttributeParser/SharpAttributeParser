namespace SharpAttributeParser;

using OneOf;

using Xunit;

public static class OneOfAssertions
{
    public static void Equal<T1, T2>(OneOf<T1, T2> expected, OneOf<T1, T2> actual) => expected.Switch
    (
        (t1) => Assert.Equal(t1, actual.AsT0),
        (t2) => Assert.Equal(t2, actual.AsT1)
    );

    public static void Equal<T1, T2, T3>(OneOf<T1, T2, T3> expected, OneOf<T1, T2, T3> actual) => expected.Switch
    (
        (t1) => Assert.Equal(t1, actual.AsT0),
        (t2) => Assert.Equal(t2, actual.AsT1),
        (t3) => Assert.Equal(t3, actual.AsT2)
    );

    public static void Equal<T1, T2>(OneOf<T1, T2>? expected, OneOf<T1, T2>? actual)
    {
        if (expected is null)
        {
            Assert.Null(actual);

            return;
        }

        if (actual is null)
        {
            Assert.Null(expected);

            return;
        }

        Equal(expected.Value, actual.Value);
    }

    public static void Equal<T1, T2, T3>(OneOf<T1, T2, T3>? expected, OneOf<T1, T2, T3>? actual)
    {
        if (expected is null)
        {
            Assert.Null(actual);

            return;
        }

        if (actual is null)
        {
            Assert.Null(expected);

            return;
        }

        Equal(expected.Value, actual.Value);
    }
}
