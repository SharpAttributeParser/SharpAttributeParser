namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>Hosts extension methods for <see cref="Action"/>, and generically overloaded types.</summary>
internal static class ActionExtensions
{
    /// <summary>Creates a <see cref="Func{T1, T2, TResult}"/> which wraps the provided <see cref="Action{T1, T2}"/> and returns <see langword="true"/>.</summary>
    /// <typeparam name="T1">The type of the first parameter of the <see cref="Action{T1, T2}"/> and <see cref="Func{T1, T2, TResult}"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the <see cref="Action{T1, T2}"/> and <see cref="Func{T1, T2, TResult}"/>.</typeparam>
    /// <param name="original">The <see cref="Action{T1, T2}"/> that is wrapped by the created <see langword="true"/>-returning <see cref="Func{T1, T2, TResult}"/>.</param>
    /// <returns>The created <see langword="true"/>-returning <see cref="Func{T1, T2, TResult}"/>.</returns>
    public static Func<T1, T2, bool> TrueReturning<T1, T2>(this Action<T1, T2> original)
    {
        return wrapper;

        bool wrapper(T1 arg1, T2 arg2)
        {
            original(arg1, arg2);

            return true;
        }
    }

    /// <summary>Creates a <see cref="Func{T1, T2, T3, TResult}"/> which wraps the provided <see cref="Action{T1, T2, T3}"/> and returns <see langword="true"/>.</summary>
    /// <typeparam name="T1">The type of the first parameter of the <see cref="Action{T1, T2, T3}"/> and <see cref="Func{T1, T2, T3, TResult}"/>.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the <see cref="Action{T1, T2, T3}"/> and <see cref="Func{T1, T2, T3, TResult}"/>.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the <see cref="Action{T1, T2, T3}"/> and <see cref="Func{T1, T2, T3, TResult}"/>.</typeparam>
    /// <param name="original">The <see cref="Action{T1, T2, T3}"/> that is wrapped by the created <see langword="true"/>-returning <see cref="Func{T1, T2, T3, TResult}"/>.</param>
    /// <returns>The created <see langword="true"/>-returning <see cref="Func{T1, T2, T3, TResult}"/>.</returns>
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
