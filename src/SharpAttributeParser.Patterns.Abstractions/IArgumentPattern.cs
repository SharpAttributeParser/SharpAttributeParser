namespace SharpAttributeParser.Patterns;

using OneOf;
using OneOf.Types;

/// <summary>Represents a pattern used to filter attribute arguments.</summary>
/// <typeparam name="T">The type of the attribute arguments matched by the pattern.</typeparam>
public interface IArgumentPattern<T>
{
    /// <summary>Attempts to fit the provided <see cref="object"/> to the pattern.</summary>
    /// <param name="argument">The <see cref="object"/> that is fit to the pattern.</param>
    /// <returns>The provided argument, fit to the pattern - or <see cref="Error"/> if the attempt was unsuccessful.</returns>
    public abstract OneOf<Error, T> TryFit(object? argument);
}
