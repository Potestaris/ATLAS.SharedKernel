namespace ATLAS.Kernel.Domain.ValueObjects;

/// <summary>
/// Represents a postal address composed of street, city, postal code, and ISO 3166-1
/// alpha-2 country code.
/// </summary>
/// <example>
/// <code>
/// var result = Address.Create("Calle Mayor 1", "Madrid", "28001", "ES");
/// if (result.IsSuccess)
/// {
///     var address = result.Value;
///     Console.WriteLine(address.City);        // "Madrid"
///     Console.WriteLine(address.CountryCode); // "ES"
/// }
/// </code>
/// </example>
public sealed class Address : ValueObject
{
    /// <summary>Gets the street name and number.</summary>
    public string Street { get; }

    /// <summary>Gets the optional second address line (apartment, floor, etc.).</summary>
    public string? Street2 { get; }

    /// <summary>Gets the city or municipality name.</summary>
    public string City { get; }

    /// <summary>Gets the state, province, or region (optional for some countries).</summary>
    public string? State { get; }

    /// <summary>Gets the postal / ZIP code.</summary>
    public string PostalCode { get; }

    /// <summary>Gets the ISO 3166-1 alpha-2 country code (e.g., <c>"ES"</c>, <c>"US"</c>).</summary>
    public string CountryCode { get; }

    private Address(string street, string? street2, string city, string? state, string postalCode, string countryCode)
    {
        Street = street;
        Street2 = street2;
        City = city;
        State = state;
        PostalCode = postalCode;
        CountryCode = countryCode;
    }

    /// <summary>
    /// Creates a validated <see cref="Address"/> value object.
    /// </summary>
    /// <param name="street">Street name and number. Required.</param>
    /// <param name="city">City name. Required.</param>
    /// <param name="postalCode">Postal or ZIP code. Required.</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code. Required (must be 2 letters).</param>
    /// <param name="street2">Optional second address line.</param>
    /// <param name="state">Optional state, province, or region.</param>
    public static Result<Address> Create(string street, string city, string postalCode, string countryCode, string? street2 = null, string? state = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            return Error.Validation("Address.Street.Empty", "Street must not be empty.");
        if (string.IsNullOrWhiteSpace(city))
            return Error.Validation("Address.City.Empty", "City must not be empty.");
        if (string.IsNullOrWhiteSpace(postalCode))
            return Error.Validation("Address.PostalCode.Empty", "Postal code must not be empty.");
        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Trim().Length != 2)
            return Error.Validation("Address.CountryCode.Invalid",
                $"Country code '{countryCode}' must be a 2-letter ISO 3166-1 alpha-2 code (e.g., 'ES', 'US').");

        return new Address(
            street.Trim(),
            street2?.Trim(),
            city.Trim(),
            state?.Trim(),
            postalCode.Trim().ToUpperInvariant(),
            countryCode.Trim().ToUpperInvariant());
    }

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street.ToUpperInvariant();
        yield return Street2?.ToUpperInvariant();
        yield return City.ToUpperInvariant();
        yield return State?.ToUpperInvariant();
        yield return PostalCode.ToUpperInvariant();
        yield return CountryCode;
    }

    /// <inheritdoc/>
    public override string ToString() => string.Join(", ", new[]
        {
            Street, Street2, City, State, PostalCode, CountryCode
        }.Where(s => !string.IsNullOrEmpty(s)));
}
