namespace ATLAS.Kernel.Domain.Result;

/// <summary>
/// Represents the outcome of an operation that returns a value of type
/// <typeparamref name="T"/> on success, using the Railway-Oriented Programming
/// pattern. Encapsulates either the success value or a failure
/// <see cref="Error"/> without using exceptions for control flow.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
/// <example>
/// <code>
/// // Returning success with a value:
/// public async Task&lt;Result&lt;CustomerDto&gt;&gt; Handle(GetCustomerByIdQuery query, CancellationToken ct)
/// {
///     var customer = await _repo.GetByIdAsync(query.CustomerId, ct);
///     if (customer is null)
///         return Error.NotFound("Customer.NotFound", $"Customer {query.CustomerId} not found.");
///     return CustomerDto.FromEntity(customer); // implicit conversion T -> Result&lt;T&gt;
/// }
///
/// // Chaining with Map / Bind:
/// Result&lt;decimal&gt; total = result
///     .Map(customer => customer.TotalPurchases)
///     .Bind(amount => amount > 0
///         ? Result&lt;decimal&gt;.Ok(amount)
///         : Error.Validation("Amount.Zero", "Amount must be positive."));
/// </code>
/// </example>
public sealed class Result<T>
{
    private readonly T? _value;

    private Result(bool isSuccess, T? value, Error error)
    {
        switch (isSuccess)
        {
            case true when error != Error.None:
                throw new InvalidOperationException("A successful result cannot carry an error.");
            case false when error == Error.None:
                throw new InvalidOperationException("A failed result must carry an error.");
        }

        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the success value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when accessed on a failed result. Check <see cref="IsSuccess"/> before accessing.
    /// </exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot access Value of a failed Result. Error: {Error}");

    /// <summary>
    /// Gets the error when <see cref="IsFailure"/> is <c>true</c>, or
    /// <see cref="Error.None"/> when the operation succeeded.
    /// </summary>
    public Error Error { get; }

    // ── Factory methods ──────────────────────────────────────────────────────

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    /// <param name="value">The success value. Must not be <c>null</c> for reference types.</param>
    public static Result<T> Ok(T value) => new(true, value, Error.None);

    /// <summary>Creates a failed result with the given error.</summary>
    public static Result<T> Fail(Error error) => new(false, default, error);

    // ── Implicit conversions ─────────────────────────────────────────────────

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a
    /// successful <see cref="Result{T}"/>.
    /// </summary>
    /// <example>
    /// <code>return customer;  // equivalent to Result&lt;Customer&gt;.Ok(customer)</code>
    /// </example>
    public static implicit operator Result<T>(T value) => Ok(value);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <example>
    /// <code>return Error.NotFound("Customer.NotFound", "Not found.");  // equivalent to Result&lt;T&gt;.Fail(…)</code>
    /// </example>
    public static implicit operator Result<T>(Error error) => Fail(error);

    // ── Railway methods ──────────────────────────────────────────────────────

    /// <summary>
    /// Projects the success value to a new type using <paramref name="mapper"/>.
    /// If the result is a failure, the error is propagated without calling <paramref name="mapper"/>.
    /// </summary>
    public Result<TOut> Map<TOut>(Func<T, TOut> mapper) => IsSuccess ? Result<TOut>.Ok(mapper(Value)) : Result<TOut>.Fail(Error);

    /// <summary>
    /// Chains a function that itself returns a <see cref="Result{TOut}"/>.
    /// If the current result is a failure, the error is propagated.
    /// </summary>
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder) => IsSuccess ? binder(Value) : Result<TOut>.Fail(Error);

    /// <summary>
    /// Projects the result to a value of type <typeparamref name="TOut"/> by
    /// executing either <paramref name="onSuccess"/> or <paramref name="onFailure"/>.
    /// </summary>
    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure) => IsSuccess ? onSuccess(Value) : onFailure(Error);

    /// <summary>Executes <paramref name="onSuccess"/> if the result is successful and returns <c>this</c>.</summary>
    public Result<T> OnSuccess(Action<T> onSuccess)
    {
        if (IsSuccess)
            onSuccess(Value);

        return this;
    }

    /// <summary>Executes <paramref name="onFailure"/> if the result is a failure and returns <c>this</c>.</summary>
    public Result<T> OnFailure(Action<Error> onFailure)
    {
        if (IsFailure)
            onFailure(Error);

        return this;
    }

    /// <summary>Converts this <see cref="Result{T}"/> to a non-generic <see cref="Result"/>.</summary>
    public Result ToResult() => IsSuccess ? Result.Ok() : Result.Fail(Error);

    /// <inheritdoc/>
    public override string ToString() =>
        IsSuccess
            ? $"Result<{typeof(T).Name}> {{ IsSuccess = true, Value = {Value} }}"
            : $"Result<{typeof(T).Name}> {{ IsFailure = true, Error = {Error} }}";
}
