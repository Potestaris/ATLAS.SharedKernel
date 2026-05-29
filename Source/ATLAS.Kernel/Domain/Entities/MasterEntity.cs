namespace ATLAS.Kernel.Domain.Entities;

/// <summary>
/// Base class for global master-data entities that are shared across all tenants
/// and must <b>never be deleted</b>.
/// </summary>
/// <remarks>
/// <para>
/// Use this base class for fundamental reference data seeded by the system:
/// <list type="bullet">
///   <item><description>Countries (ISO 3166)</description></item>
///   <item><description>Currencies (ISO 4217)</description></item>
///   <item><description>Languages / Locales (BCP-47)</description></item>
///   <item><description>Units of measure</description></item>
/// </list>
/// </para>
/// <para>
/// This class implements <see cref="IMasterData"/> and <see cref="IActivatable"/>
/// but does <b>not</b> implement <see cref="ISoftDeletable"/>. As a consequence,
/// the repository infrastructure never exposes a delete method for these entities.
/// Master entities can only be deactivated (hidden from lookups) or reactivated.
/// </para>
/// <para>
/// There is no <c>TenantId</c> — master data is global and tenant-independent.
/// </para>
/// </remarks>
/// <typeparam name="TId">The type of the primary key.</typeparam>
/// <example>
/// <code>
/// public sealed class Country : MasterEntity&lt;int&gt;
/// {
///     public string IsoAlpha2 { get; private set; } = string.Empty;
///     public string IsoAlpha3 { get; private set; } = string.Empty;
///     public string EnglishName { get; private set; } = string.Empty;
///
///     private Country() { } // EF Core
///
///     public static Country Create(int id, string alpha2, string alpha3, string name)
///         => new() { Id = id, IsoAlpha2 = alpha2, IsoAlpha3 = alpha3, EnglishName = name };
/// }
/// </code>
/// </example>
public abstract class MasterEntity<TId> : AuditableEntityBase<TId>, IMasterData, IActivatable where TId : notnull
{
    /// <inheritdoc/>
    public bool IsActive { get; private set; } = true;

    /// <summary>EF Core / serialization constructor.</summary>
    protected MasterEntity() { }

    /// <summary>Initializes with the given identifier.</summary>
    protected MasterEntity(TId id) : base(id) { }

    /// <inheritdoc/>
    public void Activate()
    {
        if (IsActive)
            return;
        IsActive = true;
    }

    /// <inheritdoc/>
    public void Deactivate()
    {
        if (!IsActive)
            return;
        IsActive = false;
    }
}
