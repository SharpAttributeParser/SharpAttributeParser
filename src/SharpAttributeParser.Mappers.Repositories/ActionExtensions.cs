namespace SharpAttributeParser.Mappers.Repositories;

using System;

internal static class ActionExtensions
{
    public static Func<T1, T2, bool> TrueReturning<T1, T2>(this Action<T1, T2> original)
    {
        return wrapper;

        bool wrapper(T1 arg1, T2 arg2)
        {
            original(arg1, arg2);

            return true;
        }
    }

    public static Func<T1, T2, T3, bool> TrueReturning<T1, T2, T3>(this Action<T1, T2, T3> original)
    {
        return wrapper;

        bool wrapper(T1 arg1, T2 arg2, T3 arg3)
        {
            original(arg1, arg2, arg3);

            return true;
        }
    }
}
