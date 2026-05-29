namespace ATLAS.Kernel.Domain.Entities;

/// <summary>
/// Base class for system-wide reference / configuration entities that are
/// shared across all tenants, are seeded by the system, and must
/// <b>never be deleted</b>.
/// </summary>
/// <remarks>
/// <para>
/// Use this base class for system-defined status values and configuration
/// lists that apply uniformly to all tenants:
/// <list type="bullet">
///   <item><description>Invoice statuses (Draft, Issued, Paid, Overdue, Void)</description></item>
///   <item><description>Order statuses (Pending, Confirmed, Shipped, Canceled)</description></item>
///   <item><description>Payment methods (BankTransfer, Card, SEPA)</description></item>
/// </list>
/// </para>
/// <para>
/// For reference data that tenants can customize or extend with their own values,
/// use <see cref="TenantReferenceEntity{TId}"/> instead.
/// </para>
/// <para>
/// Like <see cref="MasterEntity{TId}"/>, this class implements <see cref="IMasterData"/>
/// and <see cref="IActivatable"/> but not <see cref="ISoftDeletable"/>.
/// </para>
/// </remarks>
/// <typeparam name="TId">The type of the primary key.</typeparam>
/// <example>
/// <code>
/// public sealed class InvoiceStatus : ReferenceEntity&lt;int&gt;
/// {
///     public string Code { get; private set; } = string.Empty;
///     public string I18NKey { get; private set; } = string.Empty;
///
///     private InvoiceStatus() { }
///
///     public static InvoiceStatus Create(int id, string code, string i18nKey)
///         => new() { Id = id, Code = code, I18NKey = i18nKey };
///
///     // Well-known instances for use in domain logic:
///     public static readonly int DraftId   = 1;
///     public static readonly int IssuedId  = 2;
///     public static readonly int PaidId    = 3;
/// }
/// </code>
/// </example>
public abstract class ReferenceEntity<TId> : AuditableEntityBase<TId>, IMasterData, IActivatable where TId : notnull
{
    /// <inheritdoc/>
    public bool IsActive { get; private set; } = true;

    /// <summary>EF Core / serialization constructor.</summary>
    protected ReferenceEntity() { }

    /// <summary>Initializes with the given identifier.</summary>
    protected ReferenceEntity(TId id) : base(id) { }

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
