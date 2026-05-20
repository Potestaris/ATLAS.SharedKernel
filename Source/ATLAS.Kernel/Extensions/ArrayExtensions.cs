namespace ATLAS.Kernel.Extensions;

/// <summary>
/// Provides extension methods for working with arrays.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Adds an item to the array, creating a new array with increased length.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="target">The source array to which the item will be added.</param>
    /// <param name="item">The item to add to the array.</param>
    /// <param name="prepend">If <c>true</c>, adds the item at the beginning of the array; otherwise adds it at the end. Default is <c>false</c>.</param>
    /// <returns>A new array containing all elements from the source array plus the added item in the specified position.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="target"/> is null.</exception>
    public static T[] Add<T>(this T[] target, T item, bool prepend = false)
    {
        ArgumentNullException.ThrowIfNull(target);
        var result = new T[target.Length + 1];

        if (!prepend)
        {
            target.CopyTo(result, 0);
            result[target.Length] = item;
        }
        else
        {
            result[0] = item;
            Array.Copy(target, 0, result, 1, target.Length);
        }

        return result;
    }
}
