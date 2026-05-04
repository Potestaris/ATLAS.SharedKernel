namespace ATLAS.Kernel.Extensions;

public static class EnumerableResultExtensions
{
    public static Result<IReadOnlyList<T>> ToResult<T>(this IReadOnlyList<T> list) => Result<IReadOnlyList<T>>.Ok(list);

}
