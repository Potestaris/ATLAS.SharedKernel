namespace ATLAS.Kernel.Domain.Guards.Internal;

/// <summary>
/// Provides individual guard-clause methods invoked via <c>Guard.Against.*</c>.
/// This class is not intended to be used directly; use <see cref="ATLAS.Kernel.Domain.Guards.Guard"/> instead.
/// </summary>
public sealed class GuardClause
{
    internal GuardClause() { }

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> if <paramref name="value"/> is <c>null</c>.
    /// </summary>
    /// <param name="value">The value to check for null.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public T Null<T>(T? value, string parameterName) where T : class
    {
        ArgumentNullException.ThrowIfNull(value, parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is <c>null</c>
    /// or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The trimmed, non-empty string.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is null or whitespace.</exception>
    public string NullOrWhiteSpace(string? value, string parameterName)
    {
        return string.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"'{parameterName}' must not be null or whitespace.", parameterName) : value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is null or empty.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    public string NullOrEmpty(string? value, string parameterName)
    {
        return string.IsNullOrEmpty(value) ? throw new ArgumentException($"'{parameterName}' must not be null or empty.", parameterName) : value;
    }

    // ReSharper disable PossibleMultipleEnumeration
    /// <summary>
    /// Throws <see cref="ArgumentException"/> if the collection is null or contains no elements.
    /// </summary>
    /// <param name="value">The collection to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The non-empty collection.</returns>
    public IEnumerable<T> NullOrEmpty<T>(IEnumerable<T>? value, string parameterName)
    {
        if (value is null)
            throw new ArgumentException($"'{parameterName}' must not be null or empty.", parameterName);

        if (value.TryGetNonEnumeratedCount(out var count))
        {
            if (count == 0)
                throw new ArgumentException($"'{parameterName}' must not be null or empty.", parameterName);

            return value;
        }

        using var enumerator = value.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new ArgumentException($"'{parameterName}' must not be null or empty.", parameterName);

        return value;
    }
    // ReSharper restore PossibleMultipleEnumeration

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> equals the
    /// default value for its type (e.g., <c>Guid.Empty</c>, <c>0</c>).
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The non-default value.</returns>
    public T Default<T>(T value, string parameterName) where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            throw new ArgumentException(
                $"'{parameterName}' must not be the default value for type '{typeof(T).Name}'.",
                parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative (&lt; 0).
    /// </summary>
    /// <param name="value">The integer value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    /// <returns>The non-negative value.</returns>
    public int Negative(int value, string parameterName)
    {
        return value < 0 ? throw new ArgumentOutOfRangeException(parameterName, value, $"'{parameterName}' must not be negative.") : value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative (&lt; 0).
    /// </summary>
    /// <param name="value">The decimal value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    public decimal Negative(decimal value, string parameterName)
    {
        return value < 0 ? throw new ArgumentOutOfRangeException(parameterName, value, $"'{parameterName}' must not be negative.") : value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is
    /// negative or zero (&lt;= 0).
    /// </summary>
    /// <param name="value">The integer value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    public int NegativeOrZero(int value, string parameterName)
    {
        return value <= 0 ? throw new ArgumentOutOfRangeException(parameterName, value, $"'{parameterName}' must be greater than zero.") : value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is
    /// negative or zero (&lt;= 0).
    /// </summary>
    /// <param name="value">The decimal value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    public decimal NegativeOrZero(decimal value, string parameterName)
    {
        return value <= 0 ? throw new ArgumentOutOfRangeException(parameterName, value, $"'{parameterName}' must be greater than zero.") : value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is
    /// outside the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum inclusive value.</param>
    /// <param name="max">The maximum inclusive value.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    public T OutOfRange<T>(T value, T min, T max, string parameterName)
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(
                parameterName, value,
                $"'{parameterName}' must be between {min} and {max} (inclusive).");
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> exceeds
    /// <paramref name="maxLength"/> characters.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    public string MaxLength(string value, int maxLength, string parameterName)
    {
        NullOrEmpty(value, parameterName);
        if (value.Length > maxLength)
            throw new ArgumentException(
                $"'{parameterName}' must not exceed {maxLength} characters (actual: {value.Length}).",
                parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> has fewer than
    /// <paramref name="minLength"/> characters.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="minLength">The minimum required length.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    public string MinLength(string value, int minLength, string parameterName)
    {
        NullOrEmpty(value, parameterName);
        if (value.Length < minLength)
            throw new ArgumentException(
                $"'{parameterName}' must be at least {minLength} characters (actual: {value.Length}).",
                parameterName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if a value that should be positive is zero or less.
    /// Alias for <see cref="NegativeOrZero(decimal, string)"/> for readability.
    /// </summary>
    /// <param name="value">The decimal value to check.</param>
    /// <param name="parameterName">The name of the parameter being checked.</param>
    public decimal Positive(decimal value, string parameterName)
        => NegativeOrZero(value, parameterName);
}
