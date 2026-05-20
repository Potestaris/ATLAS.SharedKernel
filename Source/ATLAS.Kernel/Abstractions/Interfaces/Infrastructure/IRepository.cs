namespace ATLAS.Kernel.Abstractions.Interfaces.Infrastructure;

/// <summary>
/// Defines the base write repository contract for all ATLAS domain entities.
/// This interface intentionally does <b>not</b> expose a delete operation —
/// deletion semantics are provided by <see cref="ISoftDeletableRepository{T,TId}"/>
/// for entities implementing <c>ISoftDeletable</c>.
/// Entities implementing <c>IMasterData</c> only ever use this base contract,
/// making deletion structurally impossible at compile time.
/// </summary>
/// <typeparam name="T">The entity type. Must implement <see cref="IEntity{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the entity's primary key.</typeparam>
/// <example>
/// <code>
/// // Using the repository for a master-data entity (no delete method available):
/// IRepository&lt;Country, int&gt; repo = serviceProvider.GetRequired...();
/// await repo.AddAsync(new Country("ES", "Spain"), ct);
/// var spain = await repo.GetByIdAsync(1, ct);
/// </code>
/// </example>
public interface IRepository<T, in TId> where T : class, IEntity<TId> where TId : notnull
{
    /// <summary>
    /// Retrieves an entity by its primary key, or <c>null</c> if not found.
    /// </summary>
    /// <param name="id">The primary key value to search for.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>Returns all entities of type <typeparamref name="T"/> visible to the current tenant.</summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Schedules a new entity for insertion on the next <c>SaveChangesAsync</c> call.</summary>
    /// <param name="entity">The entity to add. Must not be null.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing entity as modified so that EF Core will emit an UPDATE statement.</summary>
    /// <param name="entity">The entity to update. Must not be null.</param>
    void Update(T entity);

    /// <summary>
    /// Determines whether an entity with the given primary key exists.
    /// More efficient than <see cref="GetByIdAsync"/> when you only need existence.
    /// </summary>
    /// <param name="id">The primary key value to check.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
}
