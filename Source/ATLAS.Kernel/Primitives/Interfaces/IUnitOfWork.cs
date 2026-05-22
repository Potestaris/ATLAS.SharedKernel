namespace ATLAS.Kernel.Primitives.Interfaces;

/// <summary>
/// Defines the unit-of-work pattern that groups multiple repository operations
/// into a single atomic transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all pending changes tracked by the repository to the database.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new explicit database transaction.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current explicit transaction.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current explicit transaction, discarding all pending changes.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends the current explicit transaction (commits if no rollback occurred).
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task EndTransactionAsync(CancellationToken cancellationToken = default);
}
