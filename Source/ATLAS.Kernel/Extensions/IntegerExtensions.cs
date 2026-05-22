using System.Globalization;

namespace ATLAS.Kernel.Extensions;

/// <summary>
/// Extension methods for <see cref="int"/> values, providing safe parsing operations.
/// </summary>
public static class IntegerExtensions
{
    /// <summary>
    /// Attempts to parse the specified string representation of a number to its 32-bit signed integer equivalent.
    /// Returns the specified default value if parsing fails due to format or overflow exceptions.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="defaultValue">The default value to return if parsing fails. If null, returns null on failure.</param>
    /// <returns>The parsed integer value, or the default value if parsing fails.</returns>
    public static int? ParseOrDefault(string s, int? defaultValue)
    {
        try
        {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            return defaultValue ?? null;
        }
        catch (OverflowException)
        {
            return defaultValue ?? null;
        }
    }
}
