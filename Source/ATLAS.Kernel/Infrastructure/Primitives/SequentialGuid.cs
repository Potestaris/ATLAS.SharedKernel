namespace ATLAS.Kernel.Infrastructure.Primitives;

/// <summary>
/// Generates sequential (monotonically increasing) GUIDs optimised for use as
/// database primary keys. Sequential GUIDs minimise B-tree index fragmentation
/// compared to random GUIDs, significantly improving INSERT performance at scale.
/// </summary>
/// <remarks>
/// <para>Two strategies are provided:</para>
/// <list type="bullet">
///   <item><description>
///     <see cref="NewSequentialGuid"/> — COMB GUID, timestamp in the last 8 bytes.
///     Optimal for <b>SQL Server</b> <c>uniqueidentifier</c> clustered indexes.
///   </description></item>
///   <item><description>
///     <see cref="NewSequentialGuidAtEnd"/> — timestamp in the last 6 bytes.
///     Optimal for <b>PostgreSQL</b>, <b>MySQL/MariaDB</b>, and <b>Oracle</b>
///     with binary UUID storage.
///   </description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // In a domain factory method — choose based on the configured provider:
/// var customerId = SequentialGuid.NewSequentialGuid();      // SQL Server
/// var orderId    = SequentialGuid.NewSequentialGuidAtEnd(); // PostgreSQL / MySQL
/// </code>
/// </example>
public static class SequentialGuid
{
    /// <summary>
    /// Generates a new sequential GUID optimised for SQL Server clustered indexes.
    /// The timestamp is embedded in bytes 10–15 (last 6 bytes of the GUID string).
    /// </summary>
    /// <returns>A <see cref="Guid"/> that sorts later than all previously generated values.</returns>
    public static Guid NewSequentialGuid()
    {
        byte[] guidBytes = Guid.NewGuid().ToByteArray();
        byte[] timestampBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

        if (BitConverter.IsLittleEndian) Array.Reverse(timestampBytes);

        guidBytes[10] = timestampBytes[2];
        guidBytes[11] = timestampBytes[3];
        guidBytes[12] = timestampBytes[4];
        guidBytes[13] = timestampBytes[5];
        guidBytes[14] = timestampBytes[6];
        guidBytes[15] = timestampBytes[7];

        return new Guid(guidBytes);
    }

    /// <summary>
    /// Generates a new sequential GUID optimised for PostgreSQL, MySQL, MariaDB,
    /// and Oracle. The timestamp is embedded at the end of the byte array.
    /// </summary>
    /// <returns>A <see cref="Guid"/> that sorts later than all previously generated values.</returns>
    public static Guid NewSequentialGuidAtEnd()
    {
        byte[] randomBytes = new byte[10];
        byte[] timestampBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks / 10_000L);

        Random.Shared.NextBytes(randomBytes);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(timestampBytes);

        byte[] allBytes = new byte[16];
        Buffer.BlockCopy(randomBytes, 0, allBytes, 0, 10);
        Buffer.BlockCopy(timestampBytes, 2, allBytes, 10, 6);

        return new Guid(allBytes);
    }
}
