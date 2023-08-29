namespace SharpAttributeParser;

using System.Collections;
using System.Collections.Generic;

internal sealed class SequenceEqualityComparer : IEqualityComparer<object?>
{
    public static IEqualityComparer<object?> Instance { get; } = new SequenceEqualityComparer();

    private SequenceEqualityComparer() { }

    private bool SequenceEquals(IEnumerable x, IEnumerable y)
    {
        if (x is null)
        {
            return y is null;
        }

        if (y is null)
        {
            return false;
        }

        var xEnumerator = x.GetEnumerator();
        var yEnumerator = y.GetEnumerator();

        while (true)
        {
            if (xEnumerator.MoveNext() is false)
            {
                return yEnumerator.MoveNext() is false;
            }

            if (yEnumerator.MoveNext() is false)
            {
                return false;
            }

            if (xEnumerator.Current is null)
            {
                if (yEnumerator.Current is null)
                {
                    continue;
                }

                return false;
            }

            if (yEnumerator.Current is null)
            {
                return false;
            }

            if (xEnumerator.Current is IEnumerable xEnumerable)
            {
                if (yEnumerator.Current is IEnumerable yEnumerable)
                {
                    if (SequenceEquals(xEnumerable, yEnumerable) is false)
                    {
                        return false;
                    }

                    continue;
                }

                return false;
            }

            if (xEnumerator.Current.Equals(yEnumerator.Current) is false)
            {
                return false;
            }
        }
    }

    bool IEqualityComparer<object?>.Equals(object? x, object? y)
    {
        if (x is IEnumerable xEnumerable)
        {
            if (y is IEnumerable yEnumerable)
            {
                return SequenceEquals(xEnumerable, yEnumerable);
            }

            return false;
        }

        if (x is null)
        {
            return y is null;
        }

        return x.Equals(y);
    }

    int IEqualityComparer<object?>.GetHashCode(object? obj) => 0;
}
