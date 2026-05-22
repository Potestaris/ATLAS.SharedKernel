using System.Globalization;

namespace ATLAS.Kernel.Extensions;

/// <summary>
/// Extension methods for <see cref="string"/> values for date parsing and formatting.
/// </summary>
/// <example>
/// <code>
/// "20260301".ToDateFromStringIso8601Format()   // 2026-03-01
/// "202603011030".ToDateFromStringWithHhmm()    // 2026-03-01T10:30:00
/// "Hello\nWorld".RemoveSpecialCharacters()     // "HelloWorld"
/// </code>
/// </example>
public static class StringExtensions
{
    /// <param name="dateAsString">The date string in yyyyMMdd format (e.g., "20260301").</param>
    extension(string? dateAsString)
    {
        /// <summary>
        /// Converts an ISO 8601 formatted date string (yyyyMMdd) to a <see cref="DateTime"/>,
        /// or returns <c>null</c> if the string is null, whitespace, or invalid.
        /// </summary>
        public DateTime? ToDateFromStringIso8601Format()
        {
            return string.IsNullOrWhiteSpace(dateAsString) ? null : ParseToDate(dateAsString, "yyyyMMdd");
        }

        /// <summary>
        /// Converts a date-time string with hours and minutes (yyyyMMddHHmm) to a <see cref="DateTime"/>,
        /// or returns <c>null</c> if the string is null, whitespace, or invalid.
        /// </summary>
        public DateTime? ToDateFromStringWithHhmm()
        {
            return string.IsNullOrWhiteSpace(dateAsString) ? null : ParseToDate(dateAsString, "yyyyMMddHHmm");
        }

        /// <summary>
        /// Removes special characters (newlines, tabs, quotes, commas, ampersands, carriage returns)
        /// from the string, returning an empty string if the input is null or whitespace.
        /// </summary>
        public string RemoveSpecialCharacters()
        {
            if (string.IsNullOrWhiteSpace(dateAsString)) return string.Empty;
            return dateAsString.Replace("\n", string.Empty, StringComparison.Ordinal)
                .Replace("\r", string.Empty, StringComparison.Ordinal)
                .Replace("\r\n", string.Empty, StringComparison.Ordinal)
                .Replace("\t", string.Empty, StringComparison.Ordinal)
                .Replace("\"", string.Empty, StringComparison.Ordinal)
                .Replace(",", string.Empty, StringComparison.Ordinal)
                .Replace("'", string.Empty, StringComparison.Ordinal)
                .Replace("&", string.Empty, StringComparison.Ordinal)
                .Replace(Environment.NewLine, string.Empty, StringComparison.Ordinal);
        }
    }

    private static DateTime? ParseToDate(string dateToParse, string format)
    {
        return DateTime.TryParseExact(dateToParse, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : null;
    }
}
