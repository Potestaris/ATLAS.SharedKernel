namespace ATLAS.Kernel.Domain.Entities;

/// <summary>
/// Base class for tenant-scoped reference / configuration entities that
/// <b>cannot be deleted</b> but can be deactivated.
/// </summary>
/// <remarks>
/// <para>
/// Use this base class for per-tenant configuration lists where:
/// <list type="bullet">
///   <item><description>The system seeds a set of global values (<see cref="IsSystemDefined"/> = <c>true</c>).</description></item>
///   <item><description>Tenants can add their own custom values (<see cref="IsSystemDefined"/> = <c>false</c>).</description></item>
///   <item><description>Neither system-defined nor tenant-defined values can ever be deleted.</description></item>
/// </list>
/// Examples: Customer statuses (Prospect, Active, Churned) which the tenant can extend
/// with custom statuses (VIP, StrategicAccount) without being able to remove the built-in ones.
/// </para>
/// <para>
/// This class implements <see cref="IMasterData"/>, <see cref="IActivatable"/>,
/// and <see cref="ITenantAware"/> but not <see cref="ISoftDeletable"/>.
/// </para>
/// </remarks>
/// <typeparam name="TId">The type of the primary key.</typeparam>
/// <example>
/// <code>
/// public sealed class CustomerStatus : TenantReferenceEntity&lt;int&gt;
/// {
///     public string Code { get; private set; } = string.Empty;
///     public string I18NKey { get; private set; } = string.Empty;
///
///     private CustomerStatus() { }
///
///     // Creates a system-defined status (seeded, non-deletable, non-deactivatable by tenants)
///     public static CustomerStatus CreateSystem(int id, string code, string i18nKey)
///         => new() { Id = id, TenantId = Guid.Empty, Code = code, I18NKey = i18nKey, IsSystemDefined = true };
///
///     // Creates a tenant-defined status (tenant can deactivate but not delete it)
///     public static CustomerStatus CreateCustom(int id, Guid tenantId, string code, string i18nKey)
///         => new() { Id = id, TenantId = tenantId, Code = code, I18NKey = i18nKey, IsSystemDefined = false };
/// }
/// </code>
/// </example>
public abstract class TenantReferenceEntity<TId> : AuditableEntityBase<TId>, IMasterData, IActivatable, ITenantAware where TId : notnull
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <inheritdoc/>
    public Guid TenantId { get; protected init; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <inheritdoc/>
    public bool IsActive { get; private set; } = true;

    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Gets a value indicating whether this reference value was seeded by the
    /// system (<c>true</c>) or created by the tenant (<c>false</c>).
    /// System-defined values cannot be deactivated by tenants.
    /// </summary>
    public bool IsSystemDefined { get; protected init; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <summary>EF Core / serialization constructor.</summary>
    protected TenantReferenceEntity() { }

    /// <summary>Initializes with the given identifier.</summary>
    protected TenantReferenceEntity(TId id) : base(id) { }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to activate an already active entity.
    /// </exception>
    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a tenant attempts to deactivate a system-defined reference value.
    /// </exception>
    public void Deactivate()
    {
        if (IsSystemDefined)
            throw new InvalidOperationException($"System-defined reference entity '{GetType().Name}' (Id={Id}) cannot be deactivated.");
        if (!IsActive)
            return;
        IsActive = false;
    }
}
