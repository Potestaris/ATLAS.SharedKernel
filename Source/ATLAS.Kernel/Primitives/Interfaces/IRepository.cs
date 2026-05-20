namespace ATLAS.Kernel.Primitives.Interfaces;

/// <summary>
/// Defines the base write repository contract for persistence operations on entities.
/// </summary>
/// <typeparam name="TEntity">The entity type. Must be a class.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    #region Commands
    /// <summary>
    /// Adds a new entity to the repository for persistence on the next <c>SaveChangesAsync</c> call.
    /// </summary>
    /// <param name="entity">The entity to add. Must not be null.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities to the repository for persistence on the next <c>SaveChangesAsync</c> call.
    /// </summary>
    /// <param name="entities">The collection of entities to add. Must not be null.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches an entity to the repository for tracking without marking it as newly added.
    /// Used for updating existing entities retrieved from an external source.
    /// </summary>
    /// <param name="entity">The entity to attach. Must not be null.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task AttachAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity as modified so that the repository will emit an UPDATE statement on the next save.
    /// </summary>
    /// <param name="entity">The entity to update. Must not be null.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity from the repository (physical delete, not soft delete).
    /// </summary>
    /// <param name="entity">The entity to delete. Must not be null.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    #endregion

    #region Queries
    /// <summary>
    /// Returns all entities of the type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>A read-only collection of all entities.</returns>
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single entity by its primary key, or <c>null</c> if not found.
    /// </summary>
    /// <param name="id">The primary key value to search for.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>The entity if found, or <c>null</c>.</returns>
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    #endregion
}
