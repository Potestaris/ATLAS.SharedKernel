namespace ATLAS.Kernel.Abstractions.Interfaces.Infrastructure;

/// <summary>
/// Defines a read-only (no-tracking) repository for use on the Query side
/// of the CQRS pattern.
/// </summary>
/// <remarks>
/// <para>
/// Queries use <see cref="IReadRepository{T,TId}"/> instead of
/// <see cref="IRepository{T,TId}"/> to prevent accidental mutations and to
/// benefit from EF Core's <c>AsNoTracking()</c> performance optimisation.
/// </para>
/// <para>
/// This interface deliberately omits <c>Add</c>, <c>Update</c>, and <c>Delete</c>.
/// </para>
/// </remarks>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TId">The type of the entity's primary key.</typeparam>
/// <example>
/// <code>
/// // In a Query handler — read-only, no-tracking:
/// public sealed class GetCustomerByIdQueryHandler
///     : IRequestHandler&lt;GetCustomerByIdQuery, Result&lt;CustomerDto&gt;&gt;
/// {
///     private readonly IReadRepository&lt;Customer, Guid&gt; _repository;
///
///     public async Task&lt;Result&lt;CustomerDto&gt;&gt; Handle(
///         GetCustomerByIdQuery request, CancellationToken ct)
///     {
///         var customer = await _repository.GetByIdAsync(request.CustomerId, ct);
///         if (customer is null)
///             return Error.NotFound("Customer.NotFound", $"Customer {request.CustomerId} not found.");
///         return CustomerDto.FromEntity(customer);
///     }
/// }
/// </code>
/// </example>
public interface IReadRepository<T, in TId> where T : class, IEntity<TId>  where TId : notnull
{
    /// <summary>
    /// Retrieves an entity by its primary key without change tracking,
    /// or <c>null</c> if not found.
    /// </summary>
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>Returns all non-deleted entities visible to the current tenant, without tracking.</summary>
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Determines whether an entity with the given key exists.</summary>
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>Returns the total count of non-deleted entities visible to the current tenant.</summary>
    Task<long> CountAsync(CancellationToken cancellationToken = default);
}
