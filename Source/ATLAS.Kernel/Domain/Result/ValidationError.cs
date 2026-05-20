namespace ATLAS.Kernel.Domain.Result;

// ReSharper disable NotAccessedPositionalProperty.Global
/// <summary>
/// Represents a single field-level validation failure, typically produced
/// by a FluentValidation validator and collected in a
/// <see cref="ValidationResult"/>.
/// </summary>
/// <param name="PropertyName">
/// The name of the property that failed validation
/// (e.g., <c>"Customer.Name"</c>, <c>"Lines[0].Quantity"</c>).
/// </param>
/// <param name="ErrorMessage">A human-readable message describing the failure.</param>
/// <param name="ErrorCode">
/// A machine-readable code for the specific rule that failed
/// (e.g., <c>"Customer.Name.MaxLength"</c>).
/// </param>
/// <param name="AttemptedValue">
/// The value that was provided and failed validation, if available.
/// </param>
/// <example>
/// <code>
/// var errors = new[]
/// {
///     new ValidationError("Name",     "Name must not be empty.",         "Customer.Name.Empty",     null),
///     new ValidationError("Email",    "Email format is invalid.",         "Customer.Email.Format",   "not-an-email"),
///     new ValidationError("Lines[0].Quantity", "Quantity must be > 0.", "Order.Line.Qty.Positive", 0),
/// };
/// return ValidationResult.WithErrors(errors);
/// </code>
/// </example>
public sealed record ValidationError(string PropertyName, string ErrorMessage, string ErrorCode, object? AttemptedValue = null);
// ReSharper restore NotAccessedPositionalProperty.Global
