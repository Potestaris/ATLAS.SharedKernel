namespace ATLAS.Kernel.Domain.ValueObjects;

/// <summary>
/// Represents an inclusive date range between a start and end date.
/// Used for subscription periods, contract durations, validity windows, etc.
/// </summary>
/// <example>
/// <code>
/// var range = DateRange.Create(
///     new DateOnly(2026, 1, 1),
///     new DateOnly(2026, 12, 31)).Value;
///
/// Console.WriteLine(range.Contains(new DateOnly(2026, 6, 15))); // true
/// Console.WriteLine(range.Duration.Days);                         // 364
///
/// // Open-ended range (no expiry):
/// var openRange = DateRange.CreateOpenEnded(new DateOnly(2026, 1, 1));
/// Console.WriteLine(openRange.IsOpenEnded); // true
/// </code>
/// </example>
public sealed class DateRange : ValueObject
{
    /// <summary>Gets the inclusive start date of the range.</summary>
    public DateOnly From { get; }

    /// <summary>
    /// Gets the inclusive end date of the range, or <c>null</c> for an open-ended range.
    /// </summary>
    public DateOnly? To { get; }

    /// <summary>Gets a value indicating whether this range has no end date.</summary>
    public bool IsOpenEnded => To is null;

    private DateRange(DateOnly from, DateOnly? to)
    {
        From = from;
        To = to;
    }

    /// <summary>Creates a bounded date range where <paramref name="from"/> ≤ <paramref name="to"/>.</summary>
    public static Result<DateRange> Create(DateOnly from, DateOnly to)
    {
        if (from > to)
            return Error.Validation("DateRange.Invalid", $"Start date ({from}) must not be after end date ({to}).");
        return new DateRange(from, to);
    }

    /// <summary>Creates an open-ended date range with no expiry date.</summary>
    public static DateRange CreateOpenEnded(DateOnly from) => new(from, null);

    /// <summary>Determines whether <paramref name="date"/> falls within this range (inclusive).</summary>
    public bool Contains(DateOnly date) =>
        date >= From && (To is null || date <= To.Value);

    /// <summary>
    /// Determines whether this range overlaps with <paramref name="other"/>.
    /// </summary>
    public bool Overlaps(DateRange other) =>
        From <= (other.To ?? DateOnly.MaxValue) &&
        (To ?? DateOnly.MaxValue) >= other.From;

    /// <summary>
    /// Gets the duration of the range, or <see cref="TimeSpan.MaxValue"/> for open-ended ranges.
    /// </summary>
    public TimeSpan Duration => To.HasValue
            ? To.Value.ToDateTime(TimeOnly.MinValue) - From.ToDateTime(TimeOnly.MinValue)
            : TimeSpan.MaxValue;

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return From;
        yield return To;
    }

    /// <inheritdoc/>
    public override string ToString() => To.HasValue ? $"{From:yyyy-MM-dd}..{To:yyyy-MM-dd}" : $"{From:yyyy-MM-dd}..∞";
}
