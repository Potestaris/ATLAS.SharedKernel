namespace ATLAS.Kernel.Domain.ValueObjects;

/// <summary>
/// Represents a percentage value between 0 and 100 (inclusive), stored as a decimal.
/// </summary>
/// <example>
/// <code>
/// var vat  = Percentage.Create(21m).Value;   // 21%
/// var disc = Percentage.Create(10m).Value;   // 10%
///
/// decimal taxAmount = vat.ApplyTo(1000m);    // 210.00
/// decimal netAmount = disc.ApplyDiscount(500m); // 450.00
///
/// Console.WriteLine(vat.AsDecimalFraction); // 0.21
/// </code>
/// </example>
public sealed class Percentage : ValueObject
{
    /// <summary>Gets the percentage value (0–100).</summary>
    public decimal Value { get; }

    /// <summary>Gets the percentage as a decimal fraction (e.g., 21% → 0.21).</summary>
    public decimal AsDecimalFraction => Value / 100m;

    private Percentage(decimal value) => Value = value;

    /// <summary>
    /// Creates a <see cref="Percentage"/> value after validating that it falls
    /// within the range 0–100 inclusive.
    /// </summary>
    /// <param name="value">The percentage value (e.g., <c>21.0</c> for 21%).</param>
    public static Result<Percentage> Create(decimal value)
    {
        if (value < 0 || value > 100)
            return Error.Validation("Percentage.OutOfRange",
                $"Percentage must be between 0 and 100 (provided: {value}).");
        return new Percentage(value);
    }

    /// <summary>Applies this percentage to <paramref name="amount"/> and returns the result amount.</summary>
    /// <example>21% of 1000 = 210</example>
    public decimal ApplyTo(decimal amount) =>
        Math.Round(amount * AsDecimalFraction, 6);

    /// <summary>Applies this percentage as a discount and returns the reduced amount.</summary>
    /// <example>10% discount on 500 = 450</example>
    public decimal ApplyDiscount(decimal amount) =>
        Math.Round(amount * (1m - AsDecimalFraction), 6);

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    /// <inheritdoc/>
    public override string ToString() => $"{Value}%";
}
