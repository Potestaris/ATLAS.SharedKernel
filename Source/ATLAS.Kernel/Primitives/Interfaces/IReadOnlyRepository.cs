namespace ATLAS.Kernel.Primitives.Interfaces;

/// <summary>
/// Defines a read-only repository interface for retrieving entities without the ability to modify them.
/// </summary>
/// <typeparam name="TEntity">The entity type. Must be a class.</typeparam>
public interface IReadOnlyRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Retrieves a single entity by its primary key, or <c>null</c> if not found.
    /// </summary>
    /// <param name="id">The primary key value to search for.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>The entity if found, or <c>null</c>.</returns>
    Task<TEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities of the type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>A read-only collection of all entities.</returns>
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
