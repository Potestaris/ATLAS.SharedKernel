using ATLAS.Kernel.Infrastructure.Pagination;

namespace ATLAS.Kernel.Infrastructure.Extensions;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/> and related collection types.
/// </summary>
/// <example>
/// <code>
/// int[] nums = [1, 2, 3, 4, 5, 6, 7];
///
/// Array.Empty&lt;int&gt;().IsNullOrEmpty()          // true
/// nums.HasItems()                              // true
/// nums.Batch(3).Count()                        // 3  ([1,2,3],[4,5,6],[7])
/// nums.ToPagedResult(PaginationRequest.Create(1, 3))  // PagedResult items=[1,2,3] total=7
/// nums.AsReadOnlyList()                        // IReadOnlyList&lt;int&gt;
/// </code>
/// </example>
public static class CollectionExtensions
{
    extension<T>(IEnumerable<T>? source)
    {
        /// <summary>Returns <c>true</c> when the sequence is <c>null</c> or has no elements.</summary>
        public bool IsNullOrEmpty() =>
            source is null || !source.Any();

        /// <summary>Returns <c>true</c> when the sequence is non-null and has at least one element.</summary>
        public bool HasItems() =>
            source is not null && source.Any();
    }

    /// <summary>
    /// Splits the sequence into batches of at most <paramref name="batchSize"/> elements.
    /// </summary>
    /// <example><code>[1,2,3,4,5].Batch(2) → [[1,2],[3,4],[5]]</code></example>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="batchSize"/> ≤ 0.</exception>
    public static IEnumerable<IReadOnlyList<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (batchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be positive.");

        var batch = new List<T>(batchSize);
        foreach (T item in source)
        {
            batch.Add(item);
            if (batch.Count < batchSize) continue;
            yield return batch.AsReadOnly();
            batch = new List<T>(batchSize);
        }
        if (batch.Count > 0) yield return batch.AsReadOnly();
    }

    /// <summary>Executes <paramref name="action"/> for every element in the sequence.</summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);
        foreach (var item in source) action(item);
    }

    /// <summary>
    /// Performs in-memory pagination. For database queries prefer EF Core <c>Skip/Take</c>.
    /// </summary>
    public static PagedResult<T> ToPagedResult<T>( this IEnumerable<T> source, PaginationRequest pagination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(pagination);
        List<T> list = source.ToList();
        List<T> items = list.Skip(pagination.Skip).Take(pagination.PageSize).ToList();
        return PagedResult<T>.Create(items, list.Count, pagination);
    }

    /// <summary>Returns the sequence as <see cref="IReadOnlyList{T}"/>, avoiding re-enumeration.</summary>
    public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> source) =>
        source is IReadOnlyList<T> list ? list : source.ToList().AsReadOnly();

    /// <summary>Returns an empty sequence when <paramref name="source"/> is <c>null</c>.</summary>
    public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T>? source) =>
        source ?? [];
}
