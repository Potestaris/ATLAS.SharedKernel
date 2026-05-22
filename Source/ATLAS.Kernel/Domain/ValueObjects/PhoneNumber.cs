namespace ATLAS.Kernel.Domain.ValueObjects;

/// <summary>
/// Represents a validated international phone number in E.164 format
/// (e.g., <c>+34612345678</c>).
/// </summary>
/// <example>
/// <code>
/// var result = PhoneNumber.Create("+34612345678");
/// Console.WriteLine(result.Value.Value);         // "+34612345678"
/// Console.WriteLine(result.Value.CountryCode);   // "+34"
///
/// var bad = PhoneNumber.Create("not-a-phone");
/// Console.WriteLine(bad.Error.Code); // "PhoneNumber.Format.Invalid"
/// </code>
/// </example>
public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex _e164 = new(
        @"^\+[1-9]\d{1,14}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(50));

    /// <summary>Gets the phone number in E.164 format (e.g., <c>+34612345678</c>).</summary>
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    /// <summary>
    /// Creates a validated <see cref="PhoneNumber"/> from an E.164-formatted string.
    /// </summary>
    /// <param name="phoneNumber">The phone number string. Must begin with <c>+</c> and contain 2–15 digits.</param>
    public static Result<PhoneNumber> Create(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Error.Validation("PhoneNumber.Empty", "Phone number must not be empty.");

        var normalised = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

        if (!_e164.IsMatch(normalised))
            return Error.Validation("PhoneNumber.Format.Invalid",
                $"'{phoneNumber}' is not a valid E.164 phone number. Expected format: +[country code][number] (e.g., +34612345678).");

        return new PhoneNumber(normalised);
    }

    /// <summary>Gets the country dialling code prefix (e.g., <c>+34</c> for Spain).</summary>
    public string CountryCode
    {
        get
        {
            // Simplified: extracts the first 2–4 digits after '+'
            // A full implementation would use a libphonenumber binding
            var digits = Value.TrimStart('+');
            return "+" + digits[..Math.Min(2, digits.Length)];
        }
    }

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
