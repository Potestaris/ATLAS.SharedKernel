namespace ATLAS.Kernel.Infrastructure.Extensions;

/// <summary>
/// Extension methods for <see cref="DateTimeOffset"/> and <see cref="DateOnly"/> values.
/// </summary>
/// <example>
/// <code>
/// var now   = DateTimeOffset.UtcNow;
/// var start = now.StartOfDay();            // 2026-03-01T00:00:00Z
/// var end   = now.EndOfDay();             // 2026-03-01T23:59:59.9999999Z
/// var epoch = now.ToUnixSeconds();        // Unix timestamp
/// bool isWE = now.IsWeekend();            // false
/// string rel = now.AddDays(-2).ToRelativeString(); // "2 days ago"
/// string iso = now.ToIso8601();           // "2026-03-01T10:30:00Z"
/// </code>
/// </example>
public static class DateTimeExtensions
{
    extension(DateTimeOffset v)
    {
        /// <summary>Returns the first instant of the same day (00:00:00.0000000).</summary>
        public DateTimeOffset StartOfDay() =>
            new(v.Year, v.Month, v.Day, 0, 0, 0, v.Offset);

        /// <summary>Returns the last instant of the same day (23:59:59.9999999).</summary>
        public DateTimeOffset EndOfDay() =>
            new DateTimeOffset(v.Year, v.Month, v.Day, 23, 59, 59, 999, v.Offset)
                .AddTicks(9999);

        /// <summary>Returns the first instant of the same month.</summary>
        public DateTimeOffset StartOfMonth() =>
            new(v.Year, v.Month, 1, 0, 0, 0, v.Offset);

        /// <summary>Returns the last instant of the same month.</summary>
        public DateTimeOffset EndOfMonth()
        {
            int last = DateTime.DaysInMonth(v.Year, v.Month);
            return new DateTimeOffset(v.Year, v.Month, last, 23, 59, 59, 999, v.Offset).AddTicks(9999);
        }

        /// <summary>Returns the Unix timestamp in seconds.</summary>
        public long ToUnixSeconds() => v.ToUnixTimeSeconds();

        /// <summary>Returns the Unix timestamp in milliseconds.</summary>
        public long ToUnixMilliseconds() => v.ToUnixTimeMilliseconds();
    }

    /// <summary>Creates a <see cref="DateTimeOffset"/> from a Unix timestamp in seconds.</summary>
    public static DateTimeOffset FromUnixSeconds(long seconds) =>
        DateTimeOffset.FromUnixTimeSeconds(seconds);

    /// <summary>Returns <c>true</c> when the date falls on Saturday or Sunday.</summary>
    public static bool IsWeekend(this DateTimeOffset v) =>
        v.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>Returns <c>true</c> when the date falls on Monday through Friday.</summary>
    public static bool IsWeekday(this DateTimeOffset v) => !v.IsWeekend();

    /// <summary>
    /// Returns a human-readable English relative string
    /// (<c>"just now"</c>, <c>"5 minutes ago"</c>, <c>"2 days from now"</c>, …).
    /// </summary>
    /// <param name="v">The reference point in time.</param>
    /// <param name="relativeTo">The "now" baseline. Defaults to <see cref="DateTimeOffset.UtcNow"/>.</param>
    public static string ToRelativeString(this DateTimeOffset v, DateTimeOffset? relativeTo = null)
    {
        DateTimeOffset now = relativeTo ?? DateTimeOffset.UtcNow;
        TimeSpan diff = now - v;
        double abs = Math.Abs(diff.TotalSeconds);
        bool past = diff.TotalSeconds >= 0;
        string Suffix(bool p) => p ? "ago" : "from now";

        return abs switch
        {
            < 5 => "just now",
            < 60 => $"{(int)abs}s {Suffix(past)}",
            < 3_600 => $"{(int)(abs / 60)}m {Suffix(past)}",
            < 86_400 => $"{(int)(abs / 3_600)}h {Suffix(past)}",
            < 2_592_000 => $"{(int)(abs / 86_400)} day{((int)(abs / 86_400) == 1 ? "" : "s")} {Suffix(past)}",
            < 31_536_000 => $"{(int)(abs / 2_592_000)} month{((int)(abs / 2_592_000) == 1 ? "" : "s")} {Suffix(past)}",
            _ => $"{(int)(abs / 31_536_000)} year{((int)(abs / 31_536_000) == 1 ? "" : "s")} {Suffix(past)}",
        };
    }

    /// <summary>
    /// Formats as ISO 8601 UTC string, e.g. <c>"2026-03-01T10:30:00Z"</c>.
    /// </summary>
    public static string ToIso8601(this DateTimeOffset v) =>
        v.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
}
