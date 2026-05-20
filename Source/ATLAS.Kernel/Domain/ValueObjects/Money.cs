namespace ATLAS.Kernel.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount paired with its ISO 4217 currency code.
/// Supports arithmetic operations between amounts in the same currency
/// and formatting according to locale conventions.
/// </summary>
/// <remarks>
/// <para>
/// Money is immutable. All arithmetic operations return a new <see cref="Money"/> instance.
/// </para>
/// <para>
/// Cross-currency operations (e.g., EUR + USD) are not permitted directly.
/// Use an <c>IExchangeRateProvider</c> in the application layer to convert
/// currencies before arithmetic.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Creating a Money value:
/// var price   = Money.Create(99.99m, "EUR").Value;
/// var tax     = Money.Create(21.00m, "EUR").Value;
/// var total   = price + tax;          // Money { Amount = 120.99, Currency = "EUR" }
///
/// // Percentage calculation:
/// var discount = price.Percentage(10m); // 10% of 99.99 = 9.999...
///
/// // Zero values:
/// var zero = Money.Zero("EUR");
///
/// // Validation:
/// var result = Money.Create(-5m, "EUR");
/// Console.WriteLine(result.IsFailure); // true — negative amounts are invalid
/// </code>
/// </example>
public sealed class Money : ValueObject
{
    /// <summary>Gets the monetary amount. Always non-negative.</summary>
    public decimal Amount { get; }

    /// <summary>Gets the ISO 4217 currency code (e.g., <c>"EUR"</c>, <c>"USD"</c>).</summary>
    public string CurrencyCode { get; }

    private Money(decimal amount, string currencyCode)
    {
        Amount = amount;
        CurrencyCode = currencyCode;
    }

    /// <summary>
    /// Creates a <see cref="Money"/> value after validating that the amount is
    /// non-negative and the currency code is a valid 3-letter ISO 4217 code.
    /// </summary>
    /// <param name="amount">The monetary amount. Must be ≥ 0.</param>
    /// <param name="currencyCode">
    /// A 3-letter ISO 4217 currency code (e.g., <c>"EUR"</c>). Case-insensitive.
    /// </param>
    public static Result<Money> Create(decimal amount, string currencyCode)
    {
        if (amount < 0)
            return Error.Validation("Money.Amount.Negative", "Monetary amount must be non-negative.");

        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3)
            return Error.Validation("Money.Currency.Invalid",
                $"Currency code '{currencyCode}' must be a 3-letter ISO 4217 code.");

        return new Money(amount, currencyCode.Trim().ToUpperInvariant());
    }

    /// <summary>Returns a zero-amount <see cref="Money"/> in the specified currency.</summary>
    public static Money Zero(string currencyCode) =>
        Create(0m, currencyCode).Value;

    // ── Arithmetic ───────────────────────────────────────────────────────────

    /// <summary>Adds two amounts in the same currency.</summary>
    /// <param name="left">The first monetary amount.</param>
    /// <param name="right">The second monetary amount.</param>
    /// <returns>A new <see cref="Money"/> instance with the sum of both amounts.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies differ.</exception>
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.CurrencyCode);
    }

    /// <summary>Subtracts <paramref name="right"/> from <paramref name="left"/> in the same currency.</summary>
    /// <param name="left">The monetary amount to subtract from.</param>
    /// <param name="right">The monetary amount to subtract.</param>
    /// <returns>A new <see cref="Money"/> instance with the difference of both amounts.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies differ or result is negative.</exception>
    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        if (left.Amount < right.Amount)
            throw new InvalidOperationException(
                $"Cannot subtract {right} from {left}: result would be negative.");
        return new Money(left.Amount - right.Amount, left.CurrencyCode);
    }

    /// <summary>Multiplies the monetary amount by a scalar factor.</summary>
    /// <param name="money">The monetary amount to multiply.</param>
    /// <param name="factor">The scalar factor to multiply by.</param>
    /// <returns>A new <see cref="Money"/> instance with the multiplied amount.</returns>
    public static Money operator *(Money money, decimal factor) =>
        new(money.Amount * factor, money.CurrencyCode);

    /// <summary>Returns this amount as the given percentage of a base amount.</summary>
    /// <param name="percentage">The percentage to apply (e.g., 21.0 for 21%).</param>
    public Money Percentage(decimal percentage) =>
        new(Math.Round(Amount * (percentage / 100m), 6), CurrencyCode);

    /// <summary>Rounds this amount to the specified number of decimal places.</summary>
    public Money Round(int decimals = 2, MidpointRounding rounding = MidpointRounding.AwayFromZero) =>
        new(Math.Round(Amount, decimals, rounding), CurrencyCode);

    // ── Comparison operators ─────────────────────────────────────────────────
    /// <summary>Determines whether <paramref name="left"/> is greater than <paramref name="right"/> in the same currency.</summary>
    /// <param name="left">The first monetary amount.</param>
    /// <param name="right">The second monetary amount.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies differ.</exception>
    public static bool operator >(Money left, Money right) { EnsureSameCurrency(left, right); return left.Amount > right.Amount; }

    /// <summary>Determines whether <paramref name="left"/> is less than <paramref name="right"/> in the same currency.</summary>
    /// <param name="left">The first monetary amount.</param>
    /// <param name="right">The second monetary amount.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies differ.</exception>
    public static bool operator <(Money left, Money right) { EnsureSameCurrency(left, right); return left.Amount < right.Amount; }

    /// <summary>Determines whether <paramref name="left"/> is greater than or equal to <paramref name="right"/> in the same currency.</summary>
    /// <param name="left">The first monetary amount.</param>
    /// <param name="right">The second monetary amount.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies differ.</exception>
    public static bool operator >=(Money left, Money right) { EnsureSameCurrency(left, right); return left.Amount >= right.Amount; }

    /// <summary>Determines whether <paramref name="left"/> is less than or equal to <paramref name="right"/> in the same currency.</summary>
    /// <param name="left">The first monetary amount.</param>
    /// <param name="right">The second monetary amount.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies differ.</exception>
    public static bool operator <=(Money left, Money right) { EnsureSameCurrency(left, right); return left.Amount <= right.Amount; }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left.CurrencyCode != right.CurrencyCode)
            throw new InvalidOperationException($"Cannot perform arithmetic between different currencies: {left.CurrencyCode} and {right.CurrencyCode}.");
    }

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return CurrencyCode;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Amount:0.00} {CurrencyCode}";
}
