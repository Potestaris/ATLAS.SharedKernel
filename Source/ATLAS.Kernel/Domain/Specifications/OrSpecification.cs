namespace ATLAS.Kernel.Domain.Specifications;

/// <summary>
/// A composite specification that is satisfied when either the left
/// or right specification is satisfied (logical OR).
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
internal sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    internal OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <inheritdoc/>
    public override Expression<Func<T, bool>> ToExpression()
    {
        Expression<Func<T, bool>> leftExpr = _left.ToExpression();
        Expression<Func<T, bool>> rightExpr = _right.ToExpression();

        ParameterExpression param = Expression.Parameter(typeof(T), "x");
        BinaryExpression body = Expression.OrElse(
            Expression.Invoke(leftExpr, param),
            Expression.Invoke(rightExpr, param));

        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
