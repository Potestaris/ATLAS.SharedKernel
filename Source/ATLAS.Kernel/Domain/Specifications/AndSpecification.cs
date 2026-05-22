namespace ATLAS.Kernel.Domain.Specifications;

/// <summary>
/// A composite specification that is satisfied when both the left
/// and right specifications are satisfied (logical AND).
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
internal sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    internal AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <inheritdoc/>
    public override Expression<Func<T, bool>> ToExpression()
    {
        Expression<Func<T, bool>> leftExpr = _left.ToExpression();
        Expression<Func<T, bool>> rightExpr = _right.ToExpression();

        // Reuse the same parameter to allow EF Core translation
        ParameterExpression param = Expression.Parameter(typeof(T), "x");
        BinaryExpression body = Expression.AndAlso(
            Expression.Invoke(leftExpr, param),
            Expression.Invoke(rightExpr, param));

        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
