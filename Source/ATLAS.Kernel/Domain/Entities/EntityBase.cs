namespace ATLAS.Kernel.Domain.Entities;

/// <summary>
/// Abstract base class for all ATLAS domain entities.
/// Provides a strongly-typed primary key and structural equality based on
/// the entity's identifier — two entity instances with the same type and
/// the same <see cref="Id"/> are considered equal regardless of reference.
/// </summary>
/// <typeparam name="TId">
/// The type of the primary key. Must be non-null.
/// Recommended: <see cref="Guid"/> generated via <c>SequentialGuid.NewSequentialGuid()</c>.
/// </typeparam>
/// <remarks>
/// <para>
/// Do not add business logic to this class. Business rules belong in concrete
/// entity or aggregate classes.
/// </para>
/// <para>
/// EF Core requires a parameterless constructor (can be private/protected) for
/// materialization. Concrete entities should provide a public static factory
/// method (<c>Create(…)</c>) as the primary creation path.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public sealed class Product : TenantEntityBase&lt;Guid&gt;
/// {
///     public string Sku { get; private set; } = string.Empty;
///
///     private Product() { }  // EF Core
///
///     public static Result&lt;Product&gt; Create(Guid id, Guid tenantId, string sku)
///     {
///         if (string.IsNullOrWhiteSpace(sku))
///             return Error.Validation("Product.Sku.Empty", "SKU must not be empty.");
///         return new Product { Id = id, TenantId = tenantId, Sku = sku };
///     }
/// }
/// </code>
/// </example>
public abstract class EntityBase<TId> : IEntity<TId> where TId : notnull
{
    /// <summary>Gets the unique identifier of this entity.</summary>
    public TId Id { get; protected init; } = default!;

    /// <summary>
    /// Initializes a new entity without setting the identifier.
    /// Required by EF Core for proxy creation; should not be used in application code.
    /// </summary>
    protected EntityBase() { }

    /// <summary>Initializes a new entity with the given identifier.</summary>
    /// <param name="id">The entity's primary key. Must not be the default value for its type.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> is equal to <c>default(TId)</c>.
    /// </exception>
    protected EntityBase(TId id)
    {
        if (EqualityComparer<TId>.Default.Equals(id, default!))
            throw new ArgumentException($"Entity id of type '{typeof(TId).Name}' must not be the default value.", nameof(id));
        Id = id;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        var other = (EntityBase<TId>)obj;

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(GetType(), Id);

    /// <summary>Determines whether two entities are equal by identity.</summary>
    public static bool operator ==(EntityBase<TId>? left, EntityBase<TId>? right) => left?.Equals(right) ?? right is null;

    /// <summary>Determines whether two entities are not equal by identity.</summary>
    public static bool operator !=(EntityBase<TId>? left, EntityBase<TId>? right) => !(left == right);
}
