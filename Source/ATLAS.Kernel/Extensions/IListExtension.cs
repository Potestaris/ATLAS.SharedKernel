namespace ATLAS.Kernel.Extensions;

// ReSharper disable InconsistentNaming
/// <summary>
/// Extension methods for <see cref="IList{T}"/> collections, providing null and empty checks.
/// </summary>
public static class IListExtension
{
    /// <summary>
    /// Determines whether the specified list is null or contains no elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="collection">The list to check.</param>
    /// <returns><c>true</c> if the list is null or empty; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty<T>(this IList<T>? collection)
    {
        return collection == null || collection.Count == 0;
    }
}
// ReSharper restore InconsistentNaming
