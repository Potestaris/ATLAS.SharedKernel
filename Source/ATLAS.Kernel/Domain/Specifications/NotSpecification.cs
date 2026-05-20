namespace ATLAS.Kernel.Domain.Specifications;

/// <summary>
/// A negation specification that is satisfied when the inner specification
/// is <b>not</b> satisfied (logical NOT).
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
internal sealed class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _inner;

    internal NotSpecification(Specification<T> inner)
        => _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    /// <inheritdoc/>
    public override Expression<Func<T, bool>> ToExpression()
    {
        Expression<Func<T, bool>> innerExpr = _inner.ToExpression();
        ParameterExpression param = Expression.Parameter(typeof(T), "x");
        UnaryExpression body = Expression.Not(Expression.Invoke(innerExpr, param));

        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
