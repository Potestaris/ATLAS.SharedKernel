namespace ATLAS.Kernel.Domain.Result;

/// <summary>
/// Represents the outcome of a multi-field validation operation,
/// collecting all <see cref="ValidationError"/> instances rather than
/// stopping at the first failure.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ValidationResult"/> is used primarily by the
/// <c>ValidationBehavior</c> pipeline behavior which collects all FluentValidation
/// errors before returning a single error response to the caller.
/// </para>
/// <para>
/// Use <see cref="Result"/> or <see cref="Result{T}"/> for single-error scenarios.
/// Use <see cref="ValidationResult"/> when multiple fields may fail simultaneously.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Creating a successful validation result:
/// var ok = ValidationResult.Success();
/// Console.WriteLine(ok.IsValid); // true
///
/// // Creating a failed validation result with multiple errors:
/// var failed = ValidationResult.WithErrors(new[]
/// {
///     new ValidationError("Name",  "Name is required.",    "Required",     null),
///     new ValidationError("Email", "Invalid email format.", "EmailFormat",  "x@"),
/// });
/// Console.WriteLine(failed.IsValid);          // false
/// Console.WriteLine(failed.Errors.Count);     // 2
///
/// // Converting to a Result for use in the handler pipeline:
/// Result result = failed.ToResult();
/// </code>
/// </example>
public sealed class ValidationResult
{
    private static readonly ValidationResult _success =
        new(Array.Empty<ValidationError>());

    private ValidationResult(IReadOnlyList<ValidationError> errors)
        => Errors = errors;

    /// <summary>Gets a value indicating whether validation succeeded (no errors).</summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>Gets the list of field-level validation errors. Empty when <see cref="IsValid"/> is <c>true</c>.</summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    /// <summary>Returns a <see cref="ValidationResult"/> representing a successful validation.</summary>
    public static ValidationResult Success() => _success;

    /// <summary>
    /// Returns a <see cref="ValidationResult"/> containing the given validation errors.
    /// </summary>
    /// <param name="errors">
    /// The validation errors. Must contain at least one element.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="errors"/> is empty.
    /// </exception>
    public static ValidationResult WithErrors(IEnumerable<ValidationError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        List<ValidationError> list = errors.ToList();

        if (list.Count == 0)
            throw new ArgumentException("Cannot create a failed ValidationResult with zero errors.", nameof(errors));

        return new ValidationResult(list);
    }

    /// <summary>
    /// Converts this <see cref="ValidationResult"/> to a <see cref="Result"/>.
    /// Returns <c>Result.Ok()</c> when valid; otherwise returns
    /// <c>Result.Fail(Error.Validation(…))</c>.
    /// </summary>
    public Result ToResult() => IsValid
        ? Result.Ok()
        : Result.Fail(Error.Validation(
            "Validation.Failed",
            $"Validation failed with {Errors.Count} error(s): {string.Join("; ", Errors.Select(e => e.ErrorMessage))}"));

    /// <summary>
    /// Returns a <see cref="Result{T}"/> failure carrying validation error information.
    /// Can only be called when <see cref="IsValid"/> is <c>false</c>.
    /// </summary>
    /// <typeparam name="T">The expected success value type.</typeparam>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="IsValid"/> is <c>true</c> — a successful validation result
    /// cannot be converted to a typed failure without a value.
    /// </exception>
    public Result<T> ToResult<T>()
    {
        if (IsValid)
            throw new InvalidOperationException(
                "Cannot convert a successful ValidationResult to Result<T>. Provide the value using Result<T>.Ok(value).");

        return Result<T>.Fail(Error.Validation(
            "Validation.Failed",
            $"Validation failed with {Errors.Count} error(s)."));
    }
}
