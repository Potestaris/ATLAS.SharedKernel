namespace ATLAS.Kernel.Domain.Entities;

/// <summary>
/// Extends <see cref="TenantEntityBase{TId}"/> to mark a class as an aggregate root
/// in the DDD sense. Aggregate roots are the sole entry points for mutations within
/// their aggregate boundary and are responsible for maintaining invariants and
/// collecting domain events.
/// </summary>
/// <remarks>
/// <para>
/// Domain events collected in <see cref="DomainEvents"/> are dispatched
/// in-process via MediatR's <c>IPublisher</c> immediately after
/// <c>AtlasDbContextBase.SaveChangesAsync</c> succeeds.
/// </para>
/// <para>
/// Never access child entities of an aggregate from outside the aggregate root;
/// all state changes must go through aggregate root methods.
/// </para>
/// </remarks>
/// <typeparam name="TId">The type of the aggregate root's primary key.</typeparam>
/// <example>
/// <code>
/// public sealed class Customer : AggregateRoot&lt;Guid&gt;
/// {
///     private readonly List&lt;Contact&gt; _contacts = new();
///     public IReadOnlyList&lt;Contact&gt; Contacts => _contacts.AsReadOnly();
///
///     public void AddContact(Contact contact)
///     {
///         Guard.Against.Null(contact);
///         _contacts.Add(contact);
///         AddDomainEvent(new ContactAddedDomainEvent(Id, contact.Id));
///     }
/// }
/// </code>
/// </example>
public abstract class AggregateRoot<TId> : TenantEntityBase<TId>, IDomainEventHolder where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <inheritdoc/>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>EF Core / serialization constructor.</summary>
    protected AggregateRoot() { }

    /// <summary>Initializes with the given identifier.</summary>
    protected AggregateRoot(TId id) : base(id) { }

    /// <inheritdoc/>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <inheritdoc/>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
