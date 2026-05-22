using System.Globalization;

namespace ATLAS.Kernel.Extensions;

/// <summary>
/// Extension methods for <see cref="DateTime"/> and nullable <see cref="DateTime"/> values
/// for date formatting and parsing operations.
/// </summary>
/// <example>
/// <code>
/// var date = new DateTime(2026, 3, 1, 10, 30, 0);
/// date.ToDateFormatWithTime()             // "20260301103000"
/// date.ToDateFormatWithTimeIso8601()      // "2026-03-01T10:30:00Z"
///
/// DateTime?.ToDateFormat()                // "20260301" or ""
/// DateTime?.ToDateFormatWithTime()        // "20260301103000" or ""
/// </code>
/// </example>
public static class DateExtensions
{
    /// <summary>
    /// Converts a nullable <see cref="DateTime"/> to ISO 8601 format (yyyyMMdd),
    /// or returns an empty string if the value is <c>null</c>.
    /// </summary>
    /// <param name="date">The date value or <c>null</c>.</param>
    public static string ToDateFormat(this DateTime? date)
    {
        return date?.ToString("yyyyMMdd", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    /// <summary>
    /// Converts a nullable <see cref="DateTime"/> to ISO 8601 format with time (yyyyMMddHHmmss),
    /// or returns an empty string if the value is <c>null</c>.
    /// </summary>
    /// <param name="date">The date value or <c>null</c>.</param>
    public static string ToDateFormatWithTime(this DateTime? date)
    {
        return date?.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to ISO 8601 format with time (yyyyMMddHHmmss).
    /// </summary>
    /// <param name="date">The date value.</param>
    public static string ToDateFormatWithTime(this DateTime date)
    {
        return date.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to ISO 8601 UTC format (yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z').
    /// </summary>
    /// <param name="date">The date value.</param>
    public static string ToDateFormatWithTimeIso8601(this DateTime date)
    {
        return date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses a portion of a string matching the specified regex pattern into a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="s">The input string to parse.</param>
    /// <param name="regex">The regex pattern to match.</param>
    /// <param name="format">The expected format of the matched date string.</param>
    /// <param name="provider">The format provider for parsing.</param>
    /// <param name="defaultValue">The value to return if parsing fails.</param>
    /// <returns>The parsed <see cref="DateTime"/>, or <paramref name="defaultValue"/> if parsing fails.</returns>
    public static DateTime? ParsePart(string s, string regex, string format, IFormatProvider provider, DateTime? defaultValue)
    {
        try
        {
            Match match = Regex.Match(s, regex);
            if (match.Success)
            {
                string value = match.Groups[0].ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    return ParseExactOrDefault(value, format, provider, defaultValue);
                }
                else
                {
                    return defaultValue ?? null;
                }
            }
            else
            {
                return defaultValue ?? null;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return defaultValue ?? null;
        }
        catch (ArgumentException)
        {
            return defaultValue ?? null;
        }
    }

    /// <summary>
    /// Parses a string in a specific format, returning a default value if parsing fails.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="format">The expected date format (e.g., <c>"yyyyMMdd"</c>).</param>
    /// <param name="provider">The culture-specific format provider.</param>
    /// <param name="defaultValue">The value to return if parsing fails.</param>
    /// <returns>The parsed <see cref="DateTime"/>, or <paramref name="defaultValue"/> if parsing fails.</returns>
    public static DateTime? ParseExactOrDefault(string s, string format, IFormatProvider provider, DateTime? defaultValue)
    {
        try
        {
            return DateTime.ParseExact(s, format, provider);
        }
        catch (FormatException)
        {
            return defaultValue ?? null;
        }
        catch (ArgumentNullException)
        {
            return defaultValue ?? null;
        }
        catch (ArgumentException)
        {
            return defaultValue ?? null;
        }
    }
}
