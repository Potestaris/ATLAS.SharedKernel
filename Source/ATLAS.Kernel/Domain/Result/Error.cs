namespace ATLAS.Kernel.Domain.Result;

/// <summary>
/// Represents a domain error with a machine-readable code, a human-readable
/// message, and a type category. Used as the failure payload of
/// <see cref="Result"/> and <see cref="Result{T}"/>.
/// </summary>
/// <param name="Code">
/// A dot-separated, PascalCase error code used by clients and logs
/// (e.g., <c>Customer.NotFound</c>, <c>Order.Stock.Insufficient</c>).
/// </param>
/// <param name="Message">A human-readable description of the error in English.</param>
/// <param name="Type">The error category used for HTTP status code mapping.</param>
/// <remarks>
/// <see cref="Error"/> is a <c>readonly record struct</c> — it is value-typed,
/// allocation-free, and structurally comparable.
/// </remarks>
/// <example>
/// <code>
/// // Using factory methods:
/// return Error.NotFound("Customer.NotFound", $"Customer {id} was not found.");
/// return Error.Validation("Customer.Name.Empty", "Customer name must not be empty.");
/// return Error.Conflict("Customer.Code.Duplicate", $"Code '{code}' already exists.");
///
/// // Checking the error type:
/// if (error.Type == ErrorType.NotFound)
///     return NotFound(error.Message);
/// </code>
/// </example>
public readonly record struct Error(string Code, string Message, ErrorType Type)
{
    /// <summary>A sentinel value that represents the absence of an error.</summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    // ── Factory methods ──────────────────────────────────────────────────────

    /// <summary>Creates a validation error.</summary>
    /// <param name="code">The machine-readable error code (e.g., <c>"Customer.Name.Empty"</c>).</param>
    /// <param name="message">A human-readable description.</param>
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

    /// <summary>Creates a not-found error.</summary>
    /// <param name="code">The machine-readable error code (e.g., <c>"Customer.NotFound"</c>).</param>
    /// <param name="message">A human-readable description.</param>
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    /// <summary>Creates a conflict error.</summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">A human-readable description.</param>
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    /// <summary>Creates a forbidden error.</summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">A human-readable description.</param>
    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);

    /// <summary>Creates an unauthorized error.</summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">A human-readable description.</param>
    public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);

    /// <summary>Creates an unexpected (internal) error.</summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">A human-readable description.</param>
    public static Error Unexpected(string code, string message) => new(code, message, ErrorType.Unexpected);

    /// <inheritdoc/>
    public override string ToString() => $"[{Type}] {Code}: {Message}";
}
