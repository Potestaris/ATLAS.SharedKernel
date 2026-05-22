using System.Reflection;
using ATLAS.Kernel.Domain.Specifications;
using ATLAS.Kernel.Infrastructure.Pagination;

namespace ATLAS.Kernel.Infrastructure.Extensions;

/// <summary>
/// Extension methods for <see cref="IQueryable{T}"/> supporting specifications,
/// dynamic sorting, and EF Core pagination.
/// </summary>
/// <example>
/// <code>
/// // In a repository — composing spec + order + paging:
/// var items = await dbContext.Customers
///     .ApplySpecification(new ActiveCustomerSpec())
///     .ApplyOrdering(pagination.SortBy, pagination.SortOrder)
///     .ApplyPaging(pagination)
///     .ToListAsync(ct);
/// </code>
/// </example>
public static class QueryableExtensions
{
    extension<T>(IQueryable<T> query)
    {
        /// <summary>
        /// Applies a <see cref="Specification{T}"/> as a <c>Where</c> predicate.
        /// The expression is translated to SQL by EF Core.
        /// </summary>
        public IQueryable<T> ApplySpecification(Specification<T> specification)
        {
            ArgumentNullException.ThrowIfNull(specification);
            return query.Where(specification.ToExpression());
        }

        /// <summary>
        /// Applies dynamic ordering based on a property name string and sort direction.
        /// No-op when <paramref name="sortBy"/> is null or whitespace.
        /// Uses reflection-based expression building; for hot paths prefer explicit ordering.
        /// </summary>
        public IQueryable<T> ApplyOrdering(string? sortBy, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (string.IsNullOrWhiteSpace(sortBy)) return query;

            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            PropertyInfo? property = typeof(T).GetProperty(sortBy,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                                                      | System.Reflection.BindingFlags.IgnoreCase);

            if (property is null)
                return query;

            MemberExpression propExpr = Expression.Property(param, property);
            LambdaExpression keyExpr = Expression.Lambda(propExpr, param);
            string method = sortOrder == SortOrder.Ascending ? "OrderBy" : "OrderByDescending";
            MethodInfo mi = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == method && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType);

            return (IQueryable<T>)mi.Invoke(null, [query, keyExpr])!;
        }

        /// <summary>Applies <c>Skip</c> and <c>Take</c> from a <see cref="PaginationRequest"/>.</summary>
        public IQueryable<T> ApplyPaging(PaginationRequest pagination)
        {
            ArgumentNullException.ThrowIfNull(pagination);
            return query.Skip(pagination.Skip).Take(pagination.PageSize);
        }
    }
}
