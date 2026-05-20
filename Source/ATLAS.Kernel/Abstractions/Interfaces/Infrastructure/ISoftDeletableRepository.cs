namespace ATLAS.Kernel.Abstractions.Interfaces.Infrastructure;

/// <summary>
/// Extends the base repository with soft-delete capability for entities that
/// implement both <see cref="IEntity{TId}"/> and <see cref="ISoftDeletable"/>.
/// </summary>
/// <remarks>
/// The <c>SoftDelete</c> method marks the entity as deleted (setting
/// <c>IsDeleted = true</c>, <c>DeletedAt</c>, and <c>DeletedBy</c>).
/// The record is <b>never</b> physically removed from the database.
/// </remarks>
/// <typeparam name="T">Entity type. Must implement both <see cref="IEntity{TId}"/> and <see cref="ISoftDeletable"/>.</typeparam>
/// <typeparam name="TId">The type of the entity's primary key.</typeparam>
/// <example>
/// <code>
/// // Soft-deleting a customer:
/// var customer = await customerRepo.GetByIdAsync(customerId, ct);
/// if (customer is null) return Result.Fail(Error.NotFound("Customer.NotFound", "..."));
/// customerRepo.SoftDelete(customer);
/// await unitOfWork.SaveChangesAsync(ct);
/// </code>
/// </example>
public interface ISoftDeletableRepository<T, in TId> : IRepository<T, TId> where T : class, IEntity<TId>, ISoftDeletable where TId : notnull
{
    /// <summary>
    /// Marks the entity as soft-deleted. The deletion timestamp and principal
    /// are populated automatically by the <c>AuditSaveChangesInterceptor</c>.
    /// </summary>
    /// <param name="entity">The entity to mark as deleted. Must not be null.</param>
    void SoftDelete(T entity);

    /// <summary>
    /// Restores a previously soft-deleted entity.
    /// </summary>
    /// <param name="entity">The entity to restore. Must not be null.</param>
    void Restore(T entity);
}
