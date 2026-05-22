namespace ATLAS.Kernel.Primitives.Interfaces;

/// <summary>
/// Extends the base <see cref="IReadOnlyRepository{TEntity}"/> interface with pagination support,
/// allowing for efficient retrieval of large result sets in manageable pages without modification capabilities.
/// </summary>
/// <typeparam name="TEntity">The entity type. Must be a class.</typeparam>
public interface IReadOnlyPagedRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Retrieves a page of entities based on the specified page number and page size.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of entities to retrieve per page.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>A read-only collection of entities on the specified page.</returns>
    Task<IReadOnlyCollection<TEntity>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    /// <summary>
    /// Returns the total count of all entities in the repository.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>The total number of entities.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
