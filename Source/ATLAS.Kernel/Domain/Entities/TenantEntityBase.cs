namespace ATLAS.Kernel.Domain.Entities;

/// <summary>
/// Base class for all tenant-scoped operational entities in ATLAS.
/// Combines tenant awareness, full auditing, and soft-delete support.
/// This is the recommended base class for entities that hold business data
/// belonging to a specific organization (Customer, Order, Invoice, etc.).
/// </summary>
/// <remarks>
/// <para>
/// The EF Core global query filter applied by <c>AtlasDbContextBase</c>
/// automatically restricts all queries to the current tenant's data via
/// <c>WHERE TenantId = @currentTenantId AND IsDeleted = 0</c>.
/// </para>
/// <para>
/// Entities inheriting from this class <b>can</b> be soft-deleted.
/// For entities that must never be deleted, see <see cref="MasterEntity{TId}"/>,
/// <see cref="ReferenceEntity{TId}"/>, or <see cref="TenantReferenceEntity{TId}"/>.
/// </para>
/// </remarks>
/// <typeparam name="TId">The type of the primary key.</typeparam>
/// <example>
/// <code>
/// public sealed class Customer : TenantEntityBase&lt;Guid&gt;
/// {
///     public string Name { get; private set; } = string.Empty;
///
///     private Customer() { } // EF Core
///
///     public static Customer Create(Guid id, Guid tenantId, string name)
///         => new() { Id = id, TenantId = tenantId, Name = name };
/// }
/// </code>
/// </example>
public abstract class TenantEntityBase<TId> : AuditableEntityBase<TId>, ITenantAware, ISoftDeletable where TId : notnull
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <inheritdoc/>
    public Guid TenantId { get; protected init; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <inheritdoc/>
    public bool IsDeleted { get; private set; }

    /// <inheritdoc/>
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <inheritdoc/>
    public string? DeletedBy { get; private set; }

    /// <summary>EF Core / serialization constructor.</summary>
    protected TenantEntityBase() { }

    /// <summary>Initializes with the given identifier.</summary>
    protected TenantEntityBase(TId id) : base(id) { }

    /// <inheritdoc/>
    public void MarkAsDeleted(string deletedBy, DateTimeOffset deletedAt)
    {
        if (string.IsNullOrWhiteSpace(deletedBy))
            throw new ArgumentException("DeletedBy must not be empty.", nameof(deletedBy));

        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
    }

    /// <inheritdoc/>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
