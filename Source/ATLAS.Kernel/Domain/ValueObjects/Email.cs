namespace ATLAS.Kernel.Domain.ValueObjects;

/// <summary>
/// Represents a validated email address.
/// </summary>
/// <remarks>
/// Validation follows a simplified RFC 5322 pattern that rejects obviously invalid
/// addresses while accepting virtually all real-world email formats.
/// The stored value is always lowercased.
/// </remarks>
/// <example>
/// <code>
/// var result = Email.Create("user@example.com");
/// if (result.IsSuccess)
///     Console.WriteLine(result.Value.Value); // "user@example.com"
///
/// var bad = Email.Create("not-an-email");
/// Console.WriteLine(bad.Error.Code); // "Email.Format.Invalid"
/// </code>
/// </example>
public sealed class Email : ValueObject
{
    private static readonly Regex _pattern = new(
        @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    /// <summary>Gets the normalised (lowercased) email address string.</summary>
    public string Value { get; }

    private Email(string value) => Value = value;

    /// <summary>
    /// Creates a validated <see cref="Email"/> value object.
    /// </summary>
    /// <param name="email">The raw email address string. Leading/trailing whitespace is trimmed.</param>
    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.Validation("Email.Empty", "Email address must not be empty.");

        string normalised = email.Trim().ToLowerInvariant();

        if (normalised.Length > 320)
            return Error.Validation("Email.TooLong", "Email address must not exceed 320 characters.");

        if (!_pattern.IsMatch(normalised))
            return Error.Validation("Email.Format.Invalid", $"'{email}' is not a valid email address.");

        return new Email(normalised);
    }

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    /// <inheritdoc/>
    public override string ToString() => Value;

    /// <summary>Implicitly converts an <see cref="Email"/> to its string representation.</summary>
    public static implicit operator string(Email email) => email.Value;
}
