namespace ATLAS.Kernel.Domain.Entities;

/// <summary>
/// Extends <see cref="EntityBase{TId}"/> with full auditing metadata:
/// who created and last modified the entity, and when.
/// </summary>
/// <remarks>
/// The <c>AuditSaveChangesInterceptor</c> in <c>ATLAS.Database</c> sets these
/// properties automatically on every save operation via <c>ICurrentUser</c> and
/// <c>IDateTimeProvider</c>. Application code should never assign them manually.
/// </remarks>
/// <typeparam name="TId">The type of the primary key.</typeparam>
public abstract class AuditableEntityBase<TId> : EntityBase<TId>, IAuditable where TId : notnull
{
    /// <inheritdoc/>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc/>
    public string CreatedBy { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public DateTimeOffset? UpdatedAt { get; private set; }

    /// <inheritdoc/>
    public string? UpdatedBy { get; private set; }

    /// <summary>EF Core / serialization constructor.</summary>
    protected AuditableEntityBase() { }

    /// <summary>Initializes with the given identifier.</summary>
    protected AuditableEntityBase(TId id) : base(id) { }

    /// <summary>
    /// Sets the creation audit fields. Called by the interceptor on first insert.
    /// </summary>
    public void SetCreated(string createdBy, DateTimeOffset createdAt)
    {
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Sets the modification audit fields. Called by the interceptor on update.
    /// </summary>
    public void SetUpdated(string updatedBy, DateTimeOffset updatedAt)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
}
